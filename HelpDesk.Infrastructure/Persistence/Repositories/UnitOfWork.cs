using HelpDesk.Application.Interfaces;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ITicketRepository Tickets { get; }
        public IUserRepository Users { get; }
        public IAuditLogRepository AuditLogs { get; }

        public UnitOfWork(AppDbContext context, ITicketRepository tickets, IUserRepository users, IAuditLogRepository auditLogs)
        {
            _context = context;
            Tickets = tickets;
            Users = users;
            AuditLogs = auditLogs;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var result = await _context.SaveChangesAsync(ct);
            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
