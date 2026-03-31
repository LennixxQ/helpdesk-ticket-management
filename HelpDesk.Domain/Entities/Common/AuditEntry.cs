namespace HelpDesk.Domain.Entities.Common
{
    public class AuditEntry
    {
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? IpAddress { get; set; }
        public List<AuditLogDetail> Details { get; set; } = new();
    }
}
