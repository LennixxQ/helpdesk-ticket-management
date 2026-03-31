using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using System.Data.Entity;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ticket>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default)
        {
            var result = await _dbSet.Where(t => t.AssignedAgentId == agentId).Include(t => t.Category).OrderByDescending(t => t.UpdatedAt).ToListAsync(ct);
            return result;
        }

        public async Task<int> GetOpenCountAsync(CancellationToken ct = default)
        {
            var result = await _dbSet.CountAsync(t => t.Status == TicketStatus.Open, ct);
            return result;
        }

        public async Task<Dictionary<TicketStatus, int>> GetStatusCountsAsync(CancellationToken ct = default)
        {
            var result = await _dbSet.GroupBy(t => t.Status).ToDictionaryAsync(g => g.Key, g => g.Count(), ct);
            return result;
        }

        public async Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, TicketStatus? status = null,
            TicketPriority? priority = null, Guid? assignedAgentId = null,
            Guid? raisedByUserId = null, string? searchTerm = null, CancellationToken ct = default)
        {
            var query = _dbSet.Include(t => t.Category).Include(t => t.RaisedByUser).Include(t => t.AssignedAgent).AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (assignedAgentId.HasValue)
                query = query.Where(t => t.AssignedAgentId == assignedAgentId.Value);

            if (raisedByUserId.HasValue)
                query = query.Where(t => t.RaisedByUserId == raisedByUserId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(term) || t.Description.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query.OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Ticket>(items, totalCount, page, pageSize);
        }

    }
}
