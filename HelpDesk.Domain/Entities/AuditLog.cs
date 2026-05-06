namespace HelpDesk.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;

        // PRD 8.3 Actor Details
        public string PerformedBy { get; set; } = string.Empty; // User ID
        public string ActorName { get; set; } = string.Empty;
        public string ActorEmail { get; set; } = string.Empty;
        public string ActorRole { get; set; } = string.Empty;

        public DateTime PerformedAt { get; set; }
        public string? IpAddress { get; set; }

        // PRD 8.3 Context
        public string? AdditionalNotes { get; set; }

        public ICollection<AuditLogDetail> Details { get; set; } = new List<AuditLogDetail>();
    }
}
