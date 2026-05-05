using HelpDesk.Application.DTOs.AuditLogQuery;
using HelpDesk.Application.DTOs.Export;
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

        public async Task<(List<AuditLogQueryDto> Items, int Total)> GetPagedAsync(DateTime? from, DateTime? to, string? actor, string? action, string? entityType, int page, int pageSize)
        {
            var query = _context.AuditLogs.Include(a => a.Details).AsQueryable();

            if (from.HasValue) query = query.Where(a => a.PerformedAt >= from.Value);
            if (to.HasValue) query = query.Where(a => a.PerformedAt <= to.Value);
            if (!string.IsNullOrEmpty(actor))
                query = query.Where(a => a.PerformedBy.Contains(actor));
            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action.Contains(action));
            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityName == entityType);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.PerformedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(a => new AuditLogQueryDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId.ToString(),
                Action = a.Action,
                PerformedBy = a.PerformedBy,
                PerformedAt = a.PerformedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                IpAddress = a.IpAddress,
                Details = a.Details.Select(d => new AuditDetailQueryDto
                {
                    FieldName = d.FieldName,
                    OldValue = d.OldValue,
                    NewValue = d.NewValue
                }).ToList()
            }).ToListAsync();

            return (items, total);
        }

        public async Task<List<AuditLogExportDto>> GetForExportAsync(DateTime from, DateTime to) =>
            await _context.AuditLogs.Include(a => a.Details).Where(a => a.PerformedAt >= from && a.PerformedAt <= to).OrderByDescending(a => a.PerformedAt).Select(a => new AuditLogExportDto
            {
                Timestamp = a.PerformedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                EntityName = a.EntityName,
                EntityId = a.EntityId.ToString(),
                Action = a.Action,
                PerformedBy = a.PerformedBy,
                IpAddress = a.IpAddress ?? "N/A",
                Changes = string.Join("; ", a.Details.Select(d => $"{d.FieldName}: '{d.OldValue ?? "null"}' → '{d.NewValue ?? "null"}'"))
            }).ToListAsync();
    }
}