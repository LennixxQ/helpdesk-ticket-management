using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Domain.Entities
{
    public class AuditLogDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public Guid AuditLogId { get; set; }
        public AuditLog AuditLog { get; set; } = null!;
    }
}
