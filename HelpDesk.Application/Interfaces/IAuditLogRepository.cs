using HelpDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog, CancellationToken ct = default);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken ct = default);
        Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    }
}
