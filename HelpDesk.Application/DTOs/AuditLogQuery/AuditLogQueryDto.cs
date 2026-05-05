namespace HelpDesk.Application.DTOs.AuditLogQuery
{
    public class AuditLogQueryDto
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public string PerformedAt { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public List<AuditDetailQueryDto> Details { get; set; } = new();
    }
}
