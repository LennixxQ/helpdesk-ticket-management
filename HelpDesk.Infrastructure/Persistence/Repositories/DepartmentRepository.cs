using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {

        public DepartmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Department?> GetByNameAsync(string name) =>
            await _context.Departments.FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());

        public async Task<IEnumerable<Department>> GetActiveAsync() =>
            await _context.Departments.Where(d => d.IsActive).Include(d => d.DepartmentHead).ToListAsync();

        public async Task<Department?> GetByIdAsync(Guid id) =>
            await _context.Departments.Include(d => d.DepartmentHead).Include(d => d.Members).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<(int ActiveUsers, int OpenTickets, int Last30Days)> GetSummaryAsync(Guid departmentId)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var activeUsers = await _context.Users.CountAsync(u => u.DepartmentId == departmentId && u.IsActive);
            var openTickets = await _context.Tickets.CountAsync(t => t.DepartmentId == departmentId && t.Status != TicketStatus.Closed);
            var last30 = await _context.Tickets.CountAsync(t => t.DepartmentId == departmentId && t.CreatedAt >= thirtyDaysAgo);
            return (activeUsers, openTickets, last30);
        }
    }
}
