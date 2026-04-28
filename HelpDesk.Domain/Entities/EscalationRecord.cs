using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class EscalationRecord : BaseEntity
{
    public Guid TicketId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public EscalationTrigger Trigger { get; set; }
    public string EscalatedBy { get; set; } = string.Empty;
    public Guid? EscalatedByUserId { get; set; }
    public DateTime EscalatedAt { get; set; } = DateTime.UtcNow;
    public Guid? AcknowledgedByUserId { get; set; }
    public DateTime? AcknowledgedAt { get; set; }


    public Ticket Ticket { get; set; } = null!;
    public User? EscalatedByUser { get; set; }
    public User? AcknowledgedByUser { get; set; }
}