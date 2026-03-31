using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context) { }

        public async Task<Ticket?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.Tickets.Include(t => t.Category).Include(t => t.RaisedByUser).Include(t => t.AssignedAgent).Include(t => t.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<PagedResult<Ticket>> GetAllPagedAsync(
            int page, int pageSize,
            TicketStatus? status = null,
            TicketPriority? priority = null,
            Guid? categoryId = null,
            Guid? agentId = null)
        {
            var query = _context.Tickets.Include(t => t.Category).Include(t => t.RaisedByUser).Include(t => t.AssignedAgent).AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (agentId.HasValue)
                query = query.Where(t => t.AssignedAgentId == agentId.Value);

            var totalCount = await query.CountAsync();

            var items = await query.OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Ticket>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(Guid userId) =>
            await _context.Tickets.Include(t => t.Category).Where(t => t.RaisedByUserId == userId).OrderByDescending(t => t.CreatedAt).ToListAsync();

        public async Task<IEnumerable<Ticket>> GetByAgentIdAsync(Guid agentId) =>
            await _context.Tickets.Include(t => t.Category).Include(t => t.RaisedByUser).Where(t => t.AssignedAgentId == agentId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();

        public async Task<int> CountByStatusAsync(TicketStatus status) =>
            await _context.Tickets.CountAsync(t => t.Status == status);

        public async Task<int> CountByPriorityAsync(TicketPriority priority) =>
            await _context.Tickets.CountAsync(t => t.Priority == priority);

        public async Task<int> CountThisMonthAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Tickets.CountAsync(t =>
                t.CreatedAt.Year == now.Year && t.CreatedAt.Month == now.Month);
        }

        public async Task<int> CountLastMonthAsync()
        {
            var last = DateTime.UtcNow.AddMonths(-1);
            return await _context.Tickets.CountAsync(t =>
                t.CreatedAt.Year == last.Year && t.CreatedAt.Month == last.Month);
        }

        public async Task<List<(Guid AgentId, string AgentName, int Count)>> GetTopAgentsThisMonthAsync(int top = 5)
        {
            var now = DateTime.UtcNow;
            return await _context.Tickets
                .Where(t =>
                    t.Status == TicketStatus.Resolved &&
                    t.AssignedAgentId != null &&
                    t.LastModifiedAt.HasValue &&
                    t.LastModifiedAt.Value.Year == now.Year &&
                    t.LastModifiedAt.Value.Month == now.Month)
                .GroupBy(t => new { t.AssignedAgentId, t.AssignedAgent!.FullName })
                .Select(g => new
                {
                    AgentId = g.Key.AssignedAgentId!.Value,
                    AgentName = g.Key.FullName,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .Select(x => ValueTuple.Create(x.AgentId, x.AgentName, x.Count))
                .ToListAsync();
        }
    }
}