using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<AuditLog>> GetByEntityIdAsync(Guid entityId) =>
            await _context.AuditLogs.Include(a => a.Details).Where(a => a.EntityId == entityId).OrderByDescending(a => a.PerformedAt).ToListAsync();
    }
}
