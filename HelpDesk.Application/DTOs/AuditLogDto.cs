namespace HelpDesk.Application.DTOs
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public List<AuditLogDetailDto> Details { get; set; } = new();
    }
}
