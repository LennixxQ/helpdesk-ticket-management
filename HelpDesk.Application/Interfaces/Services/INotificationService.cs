using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendTicketCreatedAsync(Ticket ticket, CancellationToken ct = default);
        Task SendTicketAssignedAsync(Ticket ticket, CancellationToken ct = default);
        Task SendStatusChangedAsync(Ticket ticket, string oldStatus, CancellationToken ct = default);
        Task SendCommentAddedAsync(Ticket ticket, Comment comment, CancellationToken ct = default);
        Task SendTicketEscalatedAsync(Ticket ticket, EscalationRecord escalation, CancellationToken ct = default);
        Task SendSlaWarningAsync(Ticket ticket, CancellationToken ct = default);
        Task SendSlaBreachedAsync(Ticket ticket, CancellationToken ct = default);
        Task SendTicketClosedAsync(Ticket ticket, CancellationToken ct = default);
        Task SendCsatSurveyAsync(Ticket ticket, CancellationToken ct = default);
        Task SendWelcomeAsync(User user, string tempPassword, CancellationToken ct = default);
        Task SendTestEmailAsync(Guid adminId, string toEmail, CancellationToken ct = default);
    }
}
