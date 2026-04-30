using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.Escalation
{
    public class EscalationDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public EscalationTrigger Trigger { get; set; }
        public string EscalatedByName { get; set; } = string.Empty;
        public DateTime EscalatedAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
    }
}
