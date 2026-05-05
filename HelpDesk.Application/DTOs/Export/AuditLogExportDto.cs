namespace HelpDesk.Application.DTOs.Export
{
    public class AuditLogExportDto
    {
        public string Timestamp { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Changes { get; set; } = string.Empty;
    }
}
