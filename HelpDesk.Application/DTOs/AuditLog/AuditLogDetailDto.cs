namespace HelpDesk.Application.DTOs.AuditLog
{
    public class AuditLogDetailDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
