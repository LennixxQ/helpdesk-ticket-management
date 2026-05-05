using HelpDesk.Application.DTOs.Report;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class TicketReportRepository : ITicketReportRepository
    {
        private readonly AppDbContext _context;
        public TicketReportRepository(AppDbContext context) => _context = context;

        public async Task<List<Ticket>> GetForReportAsync(ReportFilterDto filter)
        {
            var query = _context.Tickets.Include(t => t.Category).Include(t => t.RaisedByUser)
                .Include(t => t.AssignedAgent).Include(t => t.Department).Where(t => t.CreatedAt >= filter.From && t.CreatedAt <= filter.To).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Status) &&
                Enum.TryParse<TicketStatus>(filter.Status, out var status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(filter.Priority) &&
                Enum.TryParse<TicketPriority>(filter.Priority, out var priority))
                query = query.Where(t => t.Priority == priority);

            if (filter.AgentId.HasValue)
                query = query.Where(t => t.AssignedAgentId == filter.AgentId.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

            if (filter.DepartmentId.HasValue)
                query = query.Where(t => t.DepartmentId == filter.DepartmentId.Value);

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<List<Ticket>> GetArchivedPagedAsync(int page, int pageSize)
            => await _context.Tickets.IgnoreQueryFilters().Include(t => t.Category).Include(t => t.RaisedByUser)
                   .Include(t => t.AssignedAgent).Where(t => t.IsArchived && !t.IsDeleted).OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> GetArchivedCountAsync() => 
            await _context.Tickets.IgnoreQueryFilters().CountAsync(t => t.IsArchived && !t.IsDeleted);

        public async Task<int> GetCommentCountByAgentAsync(
            Guid agentId, DateTime from, DateTime to) =>
            await _context.Comments.CountAsync(c => c.UserId == agentId && c.CreatedAt >= from && c.CreatedAt <= to);
    }
}
