namespace HelpDesk.Application.DTOs
{
    public class AuditLogDetailDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
