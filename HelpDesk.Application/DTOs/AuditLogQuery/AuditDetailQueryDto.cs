namespace HelpDesk.Application.DTOs.AuditLogQuery
{
    public class AuditDetailQueryDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
