namespace HelpDesk.Domain.Entities
{
    public class AuditLogDetail
    {
        public Guid Id { get; set; }
        public Guid AuditLogId { get; set; }
        public AuditLog AuditLog { get; set; } = null!;
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
