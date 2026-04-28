namespace HelpDesk.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? IpAddress { get; set; }
        public ICollection<AuditLogDetail> Details { get; set; } = new List<AuditLogDetail>();

    }
}
