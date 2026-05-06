using HelpDesk.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationService> _logger;
        private readonly Microsoft.Extensions.DependencyInjection.IServiceScopeFactory _scopeFactory;
        private readonly ITimeZoneConverterService _timeZoneConverter;

        public NotificationService(IEmailService emailService,IEmailTemplateService templateService,IUnitOfWork uow,IConfiguration config,ILogger<NotificationService> logger, Microsoft.Extensions.DependencyInjection.IServiceScopeFactory scopeFactory, ITimeZoneConverterService timeZoneConverter)
        {
            _emailService = emailService;
            _templateService = templateService;
            _uow = uow;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _timeZoneConverter = timeZoneConverter;
        }

        private string BaseUrl => _config["AppSettings:BaseUrl"] ?? "http://localhost:4200";
        private string SystemName => _config["Smtp:FromName"] ?? "HelpDesk";

        public async Task SendTicketCreatedAsync(Ticket ticket, CancellationToken ct = default)
        {
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);
            if (raiser is null) return;

            if (!await IsEnabledAsync(raiser.Id, NotificationEventType.TicketCreated)) return;

            var model = BuildModel(ticket, raiser.FullName,$"Your ticket has been created successfully. Our support team will review it shortly.");

            await SendAsync(raiser, model, NotificationEventType.TicketCreated.ToString(), ct);
        }

        public async Task SendTicketAssignedAsync(Ticket ticket, CancellationToken ct = default)
        {
            if (ticket.AssignedAgentId is null) return;

            var agent = await _uow.Users.GetByIdAsync(ticket.AssignedAgentId.Value);
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);

            if (agent is not null && await IsEnabledAsync(agent.Id, NotificationEventType.TicketAssigned))
            {
                var model = BuildModel(ticket, agent.FullName,$"Ticket #{ticket.Id} has been assigned to you. Please review and start working on it.");
                await SendAsync(agent, model, NotificationEventType.TicketAssigned.ToString(), ct);
            }

            if (raiser is not null && await IsEnabledAsync(raiser.Id, NotificationEventType.TicketAssigned))
            {
                var model = BuildModel(ticket, raiser.FullName,$"Your ticket has been assigned to {agent?.FullName ?? "a support agent"} and is now In Progress.");
                await SendAsync(raiser, model, NotificationEventType.TicketAssigned.ToString(), ct);
            }
        }

        public async Task SendStatusChangedAsync(Ticket ticket, string oldStatus, CancellationToken ct = default)
        {
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);
            if (raiser is null) return;
            if (!await IsEnabledAsync(raiser.Id, NotificationEventType.TicketStatusChanged)) return;

            var model = BuildModel(ticket, raiser.FullName,$"Your ticket status has been updated from '{oldStatus}' to '{ticket.Status}'.");
            await SendAsync(raiser, model, NotificationEventType.TicketStatusChanged.ToString(), ct);
        }

        public async Task SendCommentAddedAsync(Ticket ticket, Comment comment, CancellationToken ct = default)
        {
            var recipients = new HashSet<Guid> { ticket.RaisedByUserId };
            if (ticket.AssignedAgentId.HasValue) recipients.Add(ticket.AssignedAgentId.Value);

            var preview = comment.Content.Length > 200 ? comment.Content[..200] + "..." : comment.Content;

            foreach (var userId in recipients)
            {
                if (!await IsEnabledAsync(userId, NotificationEventType.CommentAdded)) continue;
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user is null) continue;

                var model = BuildModel(ticket, user.FullName,$"A new comment has been added to ticket #{ticket.Id}:\n\n\"{preview}\"");
                await SendAsync(user, model, NotificationEventType.CommentAdded.ToString(), ct);
            }
        }

        public async Task SendTicketEscalatedAsync(Ticket ticket, EscalationRecord escalation, CancellationToken ct = default)
        {
            // Notify all admins
            var admins = await _uow.Users.GetByRoleAsync(UserRole.Admin);
            foreach (var admin in admins.Where(a => a.IsActive))
            {
                var model = BuildModel(ticket, admin.FullName,$"⚠️ ESCALATION: Ticket #{ticket.Id} has been escalated.\n\nReason: {escalation.Reason}\nEscalated by: {escalation.EscalatedBy}",isUrgent: true);
                await SendAsync(admin, model, NotificationEventType.TicketEscalated.ToString(), ct);
            }

            // Notify department head
            if (ticket.DepartmentId.HasValue)
            {
                var dept = await _uow.Departments.GetByIdAsync(ticket.DepartmentId.Value);
                if (dept?.DepartmentHeadId is not null)
                {
                    var head = await _uow.Users.GetByIdAsync(dept.DepartmentHeadId.Value);
                    if (head is not null)
                    {
                        var model = BuildModel(ticket, head.FullName,$"⚠️ A ticket from your department has been escalated.\n\nReason: {escalation.Reason}",isUrgent: true);
                        await SendAsync(head, model, NotificationEventType.TicketEscalated.ToString(), ct);
                    }
                }
            }
        }

        public async Task SendSlaWarningAsync(Ticket ticket, CancellationToken ct = default)
        {
            var recipients = new List<User>();

            if (ticket.AssignedAgentId.HasValue)
            {
                var agent = await _uow.Users.GetByIdAsync(ticket.AssignedAgentId.Value);
                if (agent is not null) recipients.Add(agent);
            }

            var admins = await _uow.Users.GetByRoleAsync(UserRole.Admin);
            recipients.AddRange(admins.Where(a => a.IsActive));

            foreach (var user in recipients)
            {
                var model = BuildModel(ticket, user.FullName,$"⏰ SLA Warning: Ticket #{ticket.Id} has consumed 75%+ of its SLA time.\n\nDeadline: {ticket.SlaDeadline:dd MMM yyyy, HH:mm} UTC\nPlease take action before breach.",isUrgent: true);
                await SendAsync(user, model, NotificationEventType.SlaWarning.ToString(), ct);
            }
        }

        public async Task SendSlaBreachedAsync(Ticket ticket, CancellationToken ct = default)
        {
            var recipients = new List<User>();

            // PRD Update: Do not notify the end-user of SLA breaches to protect internal metrics.

            if (ticket.AssignedAgentId.HasValue)
            {
                var agent = await _uow.Users.GetByIdAsync(ticket.AssignedAgentId.Value);
                if (agent is not null) recipients.Add(agent);
            }

            var admins = await _uow.Users.GetByRoleAsync(UserRole.Admin);
            recipients.AddRange(admins.Where(a => a.IsActive));

            foreach (var user in recipients.DistinctBy(u => u.Id))
            {
                var model = BuildModel(ticket, user.FullName,$"🚨 SLA BREACHED: Ticket #{ticket.Id} has exceeded its resolution deadline.",isUrgent: true);
                await SendAsync(user, model, NotificationEventType.SlaBreached.ToString(), ct);
            }
        }

        public async Task SendTicketClosedAsync(Ticket ticket, CancellationToken ct = default)
        {
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);
            if (raiser is null) return;
            if (!await IsEnabledAsync(raiser.Id, NotificationEventType.TicketClosed)) return;

            var model = BuildModel(ticket, raiser.FullName, $"Your ticket #{ticket.Id.ToString()[..8]} has been closed. Thank you for your patience.");
            await SendAsync(raiser, model, NotificationEventType.TicketClosed.ToString(), ct);
        }

        public async Task SendTicketReopenedAsync(Ticket ticket, string reopenedBy, CancellationToken ct = default)
        {
            var recipients = new HashSet<Guid>();
            
            // If assigned, notify the agent
            if (ticket.AssignedAgentId.HasValue)
            {
                recipients.Add(ticket.AssignedAgentId.Value);
            }
            else
            {
                // If not assigned, notify all active admins
                var admins = await _uow.Users.GetByRoleAsync(UserRole.Admin);
                foreach (var admin in admins.Where(a => a.IsActive))
                {
                    recipients.Add(admin.Id);
                }
            }

            foreach (var userId in recipients)
            {
                if (!await IsEnabledAsync(userId, NotificationEventType.TicketReopened)) continue;
                
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user is null) continue;

                var model = BuildModel(ticket, user.FullName, $"Ticket #{ticket.Id} has been REOPENED by {reopenedBy}. Please review the ticket.");
                await SendAsync(user, model, NotificationEventType.TicketReopened.ToString(), ct);
            }

            // Also notify the raiser that it was successfully reopened (if they didn't reopen it themselves)
            // Or just always notify the raiser
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);
            if (raiser is not null && await IsEnabledAsync(raiser.Id, NotificationEventType.TicketReopened))
            {
                var model = BuildModel(ticket, raiser.FullName, $"Your ticket #{ticket.Id} has been successfully reopened.");
                await SendAsync(raiser, model, NotificationEventType.TicketReopened.ToString(), ct);
            }
        }

        public async Task SendCsatSurveyAsync(Ticket ticket, CancellationToken ct = default)
        {
            var raiser = await _uow.Users.GetByIdAsync(ticket.RaisedByUserId);
            if (raiser is null) return;
            if (!await IsEnabledAsync(raiser.Id, NotificationEventType.SurveyRequest)) return;

            var surveyUrl = $"{BaseUrl}/csat/{ticket.Id}";
            var model = BuildModel(ticket, raiser.FullName, "Your ticket has been resolved! We'd love to hear your feedback.\n\nPlease take 30 seconds to rate your experience.");
            model.TicketUrl = surveyUrl;

            // For testing: 1 minute delay for survey email (PRD specifies 30m)
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), ct);
                    var html = await _templateService.RenderCsatSurveyAsync(model);
                    await SendAsync(raiser, model, NotificationEventType.SurveyRequest.ToString(), ct, html);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Delayed CSAT survey send failed for ticket {TicketId}", ticket.Id);
                }
            });
        }

        public async Task SendWelcomeAsync(User user, string tempPassword, CancellationToken ct = default)
        {
            var model = new WelcomeEmailModel
            {
                SystemName = SystemName,
                RecipientName = user.FullName,
                Email = user.Email!,
                TempPassword = tempPassword,
                LoginUrl = $"{BaseUrl}/login"
            };

            var html = await _templateService.RenderWelcomeAsync(model);

            await _emailService.SendAsync(new EmailMessage
            {
                RecipientUserId = user.Id,
                ToEmail = user.Email!,
                Subject = $"Welcome to {SystemName}",
                HtmlBody = html,
                PlainTextBody = $"Welcome {user.FullName}! Your account has been created. Email: {user.Email} | Password: {tempPassword} | Login: {BaseUrl}/login",
                EventType = NotificationEventType.AccountCreated.ToString()
            }, ct);
        }

        public async Task SendPasswordChangedAsync(User user, CancellationToken ct = default)
        {
            var model = new SecurityEmailModel
            {
                SystemName = SystemName,
                RecipientName = user.FullName,
                ActionTimestamp = _timeZoneConverter.ConvertToLocal(DateTime.UtcNow),
                TimeZoneName = _timeZoneConverter.GetTimeZoneAbbreviation()
            };

            var html = await _templateService.RenderPasswordChangedAsync(model);

            await _emailService.SendAsync(new EmailMessage
            {
                RecipientUserId = user.Id,
                ToEmail = user.Email!,
                Subject = $"Security Alert: Your {SystemName} password was changed",
                HtmlBody = html,
                PlainTextBody = $"Hello {user.FullName}, your {SystemName} password was changed on {model.ActionTimestamp:f} {model.TimeZoneName}. If you did not do this, contact support immediately.",
                EventType = NotificationEventType.PasswordChanged.ToString()
            }, ct);
        }

        public async Task SendTestEmailAsync(Guid adminId, string toEmail, CancellationToken ct = default)
        {
            await _emailService.SendAsync(new EmailMessage
            {
                RecipientUserId = adminId,
                ToEmail = toEmail,
                Subject = $"{SystemName} — Test Email",
                HtmlBody = $"<html><body><h2>{SystemName}</h2><p>This is a test email. Your SMTP configuration is working correctly! ✅</p></body></html>",
                PlainTextBody = $"{SystemName} — Test email. SMTP configuration is working correctly.",
                EventType = "TestEmail"
            }, ct);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private TicketEmailModel BuildModel(Ticket ticket,string recipientName,string messageBody,bool isUrgent = false)
        {
            var statusClass = ticket.Status.ToString().ToLower().Replace("inprogress", "inprogress");
            var model = new TicketEmailModel
            {
                SystemName = SystemName,
                RecipientName = recipientName,
                MessageBody = messageBody,
                TicketId = ticket.Id.ToString()[..8],
                TicketTitle = ticket.Title,
                Status = ticket.Status.ToString(),
                StatusClass = statusClass,
                Priority = ticket.Priority.ToString(),
                Category = ticket.Category?.Name ?? "N/A",
                AssignedAgent = ticket.AssignedAgent?.FullName,
                TicketUrl = $"{BaseUrl}/api/tickets/view/{ticket.Id}",
                PreferencesUrl = $"{BaseUrl}/profile/notifications",
                TimeZoneName = _timeZoneConverter.GetTimeZoneAbbreviation()
            };

            if (ticket.SlaDeadline.HasValue)
            {
                model.SlaDeadline = _timeZoneConverter.ConvertToLocal(ticket.SlaDeadline.Value);
            }
            
            return model;
        }

        private async Task SendAsync(
            User user,
            TicketEmailModel model,
            string eventType,
            CancellationToken ct,
            string? htmlOverride = null)
        {
            var html = htmlOverride ?? await _templateService.RenderTicketEmailAsync(model);
            var plainText = _templateService.RenderPlainText(model);

            // Fire and forget — don't await to keep API response fast
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var scopedEmailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await scopedEmailService.SendAsync(new EmailMessage
                    {
                        RecipientUserId = user.Id,
                        ToEmail = user.Email!,
                        Subject = $"[{model.SystemName}] {model.TicketTitle} — {model.Status}",
                        HtmlBody = html,
                        PlainTextBody = plainText,
                        EventType = eventType
                    }, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background email send failed for {Email}", user.Email);
                }
            });
        }

        private async Task<bool> IsEnabledAsync(Guid userId, NotificationEventType eventType) => 
            await _uow.NotificationPreferences.IsEnabledAsync(userId, eventType);
    }
}
