using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.BackgroundServices;

public class SlaMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SlaMonitorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public SlaMonitorService(IServiceScopeFactory scopeFactory, ILogger<SlaMonitorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Monitor Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);

            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SLA monitor cycle.");
            }
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var businessHoursService = scope.ServiceProvider.GetRequiredService<IBusinessHoursService>();

        var now = DateTime.UtcNow;

        // ── Handle SLA breaches ─────────────────────────────────────────
        var breaching = await uow.Sla.GetBreachingRecordsAsync();
        foreach (var record in breaching)
        {
            // Only breach during business hours — pause timer outside
            if (!businessHoursService.IsBusinessHour(now))
            {
                _logger.LogDebug("Skipping SLA breach check for ticket {TicketId} — outside business hours.",record.TicketId);
                continue;
            }

            record.IsBreached = true;
            record.BreachedAt = now;
            record.Status = SlaStatus.Breached;
            record.Ticket.SlaBreached = true;
            record.Ticket.SlaStatus = SlaStatus.Breached;

            if (!record.Ticket.IsEscalated)
            {
                record.Ticket.IsEscalated = true;
                record.Ticket.LastModifiedAt = now;
                record.Ticket.EscalationRecord = new Domain.Entities.EscalationRecord
                {
                    TicketId = record.Ticket.Id,
                    Reason = "System Auto-Escalation: SLA Breached.",
                    Trigger = Domain.Enums.EscalationTrigger.SlaBreached,
                    EscalatedBy = "System",
                    EscalatedByUserId = null,
                    EscalatedAt = now,
                    CreatedAt = now,
                    CreatedBy = "System"
                };
            }


            _logger.LogWarning("SLA BREACHED — TicketId: {TicketId} | Priority: {Priority} | Deadline was: {Deadline}",record.TicketId,record.Ticket.Priority,record.SlaDeadline);
        }

        // ── Handle SLA warnings (75%+ elapsed) ─────────────────────────
        var warnings = await uow.Sla.GetWarningRecordsAsync();
        foreach (var record in warnings)
        {
            if (!businessHoursService.IsBusinessHour(now)) continue;

            record.Status = SlaStatus.Warning;
            record.Ticket.SlaStatus = SlaStatus.Warning;


            _logger.LogInformation("SLA Warning — TicketId: {TicketId} | 75%+ elapsed | Deadline: {Deadline}",record.TicketId,record.SlaDeadline);
        }

        // ── Auto-escalate: Critical unassigned > 30 min ─────────────────
        await AutoEscalateCriticalUnassignedAsync(uow, businessHoursService, now, ct);

        // ── Auto-escalate: OnHold > 3 business days ─────────────────────
        await AutoEscalateOnHoldAsync(uow, businessHoursService, now, ct);

        await uow.SaveChangesAsync();
    }

    private async Task AutoEscalateCriticalUnassignedAsync(IUnitOfWork uow,IBusinessHoursService bhs,DateTime now,CancellationToken ct)
    {
        if (!bhs.IsBusinessHour(now)) return;

        var allTickets = await uow.Tickets.GetAllAsync();

        var criticalUnassigned = allTickets.Where(t => t.Priority == TicketPriority.Critical && t.Status == TicketStatus.Open && t.AssignedAgentId == null && !t.IsEscalated &&(now - t.CreatedAt).TotalMinutes >= 30);

        foreach (var ticket in criticalUnassigned)
        {
            ticket.IsEscalated = true;
            ticket.LastModifiedAt = now;

            ticket.EscalationRecord = new Domain.Entities.EscalationRecord
            {
                TicketId = ticket.Id,
                Reason = "System Auto-Escalation: Critical ticket unassigned for 30+ minutes.",
                Trigger = Domain.Enums.EscalationTrigger.CriticalNotAssigned,
                EscalatedBy = "System",
                EscalatedByUserId = null,
                EscalatedAt = now,
                CreatedAt = now,
                CreatedBy = "Help Desk"
            };


            _logger.LogWarning("AUTO-ESCALATED (Critical Unassigned) — TicketId: {TicketId} | Created: {CreatedAt}",ticket.Id, ticket.CreatedAt);
        }
    }

    private async Task AutoEscalateOnHoldAsync(IUnitOfWork uow,IBusinessHoursService bhs,DateTime now,CancellationToken ct)
    {
        if (!bhs.IsBusinessHour(now))
            return;

        var allTickets = await uow.Tickets.GetAllAsync();

        // 3 business days = 3 * 8 hours * 60 min = 1440 business minutes
        var onHoldTickets = allTickets.Where(t => t.Status == Domain.Enums.TicketStatus.OnHold && !t.IsEscalated && t.LastModifiedAt.HasValue);

        foreach (var ticket in onHoldTickets)
        {
            // Calculate business minutes elapsed since going OnHold
            var businessMinsElapsed = CalculateBusinessMinutes(bhs, ticket.LastModifiedAt!.Value, now);

            if (businessMinsElapsed >= 1440) // 3 business days
            {
                ticket.IsEscalated = true;
                ticket.LastModifiedAt = now;

                ticket.EscalationRecord = new Domain.Entities.EscalationRecord
                {
                    TicketId = ticket.Id,
                    Reason = "System Auto-Escalation: Ticket on hold for 3+ business days.",
                    Trigger = Domain.Enums.EscalationTrigger.OnHoldTooLong,
                    EscalatedBy = "System",
                    EscalatedByUserId = null,
                    EscalatedAt = now,
                    CreatedAt = now,
                    CreatedBy = "system"
                };


                _logger.LogWarning("AUTO-ESCALATED (OnHold Too Long) — TicketId: {TicketId} | OnHold since: {Since}",ticket.Id, ticket.LastModifiedAt);
            }
        }
    }

    private static int CalculateBusinessMinutes(IBusinessHoursService bhs,DateTime from,DateTime to)
    {
        if (from >= to) return 0;

        var count = 0;
        var current = from;

        // Step through every 1 minute — pragmatic for background service
        while (current < to)
        {
            if (bhs.IsBusinessHour(current)) count++;
            current = current.AddMinutes(1);
        }

        return count;
    }
}