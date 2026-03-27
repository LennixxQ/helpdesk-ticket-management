using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using System.Data.Entity;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
        {
            var logDetails = await _context.AuditLogs.AddAsync(auditLog, ct);
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken ct = default)
        {
            var getByEntity = await _context.AuditLogs.Include(a => a.Details).Include(a => a.PerformedByUser).Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.PerformedAt).ToListAsync(ct);

            return getByEntity;
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        {
            var getByUser = await _context.AuditLogs.Include(a => a.Details).Where(a => a.PerformedByUserId == userId).OrderByDescending(a => a.PerformedAt).ToListAsync(ct);
            return getByUser;
        }
    }
}
