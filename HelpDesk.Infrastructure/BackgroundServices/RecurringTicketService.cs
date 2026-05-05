using Cronos;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.BackgroundServices;

public class RecurringTicketService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecurringTicketService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    public RecurringTicketService(IServiceScopeFactory scopeFactory, ILogger<RecurringTicketService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Recurring Ticket Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);

            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in recurring ticket cycle.");
            }
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var bhs = scope.ServiceProvider.GetRequiredService<IBusinessHoursService>();

        var now = DateTime.UtcNow;
        var dueTemplates = await uow.RecurringTemplates.GetDueTemplatesAsync();

        if (!dueTemplates.Any())
        {
            _logger.LogDebug("No due recurring templates at {Time}", now);
            return;
        }

        foreach (var template in dueTemplates)
        {
            try
            {
                var scheduledAt = template.NextRunAt!.Value;

                // If today is a holiday/weekend, defer to next business day start
                var fireTime = bhs.IsWorkingDay(DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(now,TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")))) ? now : bhs.GetNextBusinessStart(now);

                // If deferred, update NextRunAt and skip for now
                if (fireTime > now)
                {
                    _logger.LogInformation("Template '{Name}' deferred to next business day: {FireTime}",template.TemplateName, fireTime);

                    template.NextRunAt = fireTime;
                    template.LastModifiedAt = now;
                    uow.RecurringTemplates.Update(template);
                    await uow.SaveChangesAsync();
                    continue;
                }

                // Create ticket
                var ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = template.TicketTitle,
                    Description = template.Description,
                    CategoryId = template.CategoryId,
                    Priority = template.Priority,
                    Status = TicketStatus.Open,
                    RaisedByUserId = template.RaiseOnBehalfOfId,
                    AssignedAgentId = template.AssignToAgentId,
                    CreatedAt = now,
                    CreatedBy = "system"
                };
                await uow.Tickets.AddAsync(ticket);
                await uow.SaveChangesAsync();

                // Create SLA record — use business hours aware deadline
                var policy = await uow.Sla.GetPolicyByPriorityAsync(template.Priority);
                if (policy is not null)
                {
                    var deadline = bhs.AddBusinessMinutes(now, policy.ResolutionMinutes);
                    await uow.Sla.AddAsync(new SlaRecord
                    {
                        Id = Guid.NewGuid(),
                        TicketId = ticket.Id,
                        SlaDeadline = deadline,
                        Status = SlaStatus.WithinSla,
                        CreatedAt = now,
                        CreatedBy = "system"
                    });
                    ticket.SlaDeadline = deadline;
                    uow.Tickets.Update(ticket);
                }

                // Log the run
                template.Runs.Add(new RecurringTemplateRun
                {
                    Id = Guid.NewGuid(),
                    TemplateId = template.Id,
                    GeneratedTicketId = ticket.Id,
                    ScheduledAt = scheduledAt,
                    ActualRunAt = now,
                    IsSuccess = true
                });

                // Calculate next run — respecting business calendar
                template.RunCount++;
                template.LastRunAt = now;
                template.NextRunAt = CalculateNextRun(template, bhs, now);
                template.LastModifiedAt = now;
                uow.RecurringTemplates.Update(template);

                await uow.SaveChangesAsync();

                _logger.LogInformation(
                    "Recurring template '{Name}' fired — Ticket {TicketId} created. Next run: {NextRun}",
                    template.TemplateName, ticket.Id, template.NextRunAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fire template '{Name}'.", template.TemplateName);

                // Log failed run
                template.Runs.Add(new RecurringTemplateRun
                {
                    Id = Guid.NewGuid(),
                    TemplateId = template.Id,
                    ScheduledAt = template.NextRunAt ?? DateTime.UtcNow,
                    ActualRunAt = DateTime.UtcNow,
                    IsSuccess = false,
                    FailureReason = ex.Message
                });
                uow.RecurringTemplates.Update(template);
                await uow.SaveChangesAsync();
            }
        }
    }

    private static DateTime? CalculateNextRun(RecurringTemplate template,IBusinessHoursService bhs,DateTime from)
    {
        DateTime? raw = template.RecurrencePattern switch
        {
            RecurrencePattern.Daily => from.AddDays(1),
            RecurrencePattern.Weekly => from.AddDays(7),
            RecurrencePattern.Monthly => from.AddMonths(1),
            RecurrencePattern.Custom => ParseCronNext(template.CronExpression, from),
            _ => null
        };

        if (raw is null) return null;

        // Shift to next business day start if landing on holiday/weekend
        var ist = TimeZoneInfo.ConvertTimeFromUtc(raw.Value,TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        var dateOnly = DateOnly.FromDateTime(ist);

        if (!bhs.IsWorkingDay(dateOnly))
        {
            var nextStart = bhs.GetNextBusinessStart(raw.Value);
            return nextStart;
        }

        return raw;
    }

    private static DateTime? ParseCronNext(string? cronExpression, DateTime from)
    {
        if (string.IsNullOrWhiteSpace(cronExpression)) return null;

        try
        {
            var cron = CronExpression.Parse(cronExpression);
            return cron.GetNextOccurrence(from, TimeZoneInfo.Utc);
        }
        catch
        {
            return null;
        }
    }
}