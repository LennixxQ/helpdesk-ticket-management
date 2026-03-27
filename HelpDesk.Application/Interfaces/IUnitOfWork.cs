using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        IAuditLogRepository AuditLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
