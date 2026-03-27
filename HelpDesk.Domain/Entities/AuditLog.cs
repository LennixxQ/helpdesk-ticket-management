using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public Guid PerformedByUserId { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        public string? IPAddress { get; set; }
        public User PerformedByUser { get; set; } = null!;
        public ICollection<AuditLogDetail> Details { get; set; } = [];
    }
}
