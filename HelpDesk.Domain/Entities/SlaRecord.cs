using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities
{
    public class SlaRecord : BaseEntity
    {
        public Guid TicketId { get; set; }
        public DateTime SlaDeadline { get; set; }
        public DateTime? PausedAt { get; set; }
        public int TotalPausedMinutes { get; set; } = 0;
        public SlaStatus Status { get; set; } = SlaStatus.WithinSla;
        public bool IsBreached { get; set; } = false;
        public DateTime? BreachedAt { get; set; }
        public bool IsOverridden { get; set; } = false;
        public Guid? OverriddenById { get; set; }
        public string? OverrideReason { get; set; }


        public Ticket Ticket { get; set; } = null!;
    }
}
