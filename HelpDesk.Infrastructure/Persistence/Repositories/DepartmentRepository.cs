using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Department department) => 
            await _context.Departments.AddAsync(department);

        public async Task<IEnumerable<Department>> GetActiveAsync() =>
            await _context.Departments.Where(d => d.IsActive).Include(d => d.DepartmentHead).ToListAsync();

        public async Task<IEnumerable<Department>> GetAllAsync() 
            => await _context.Departments.Include(d => d.DepartmentHead).ToListAsync();

        public async Task<Department?> GetByIdAsync(Guid id) =>
            await _context.Departments.Include(d => d.DepartmentHead).Include(d => d.Members).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<DepartmentSummary> GetSummaryAsync(Guid departmentId)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return new DepartmentSummary
            {
                ActiveUserCount = await _context.Users.CountAsync(u => u.DepartmentId == departmentId && u.IsActive),
                OpenTicketCount = await _context.Tickets.CountAsync(t => t.DepartmentId == departmentId && t.Status != TicketStatus.Closed),
                TicketsLast30Days = await _context.Tickets.CountAsync(t => t.DepartmentId == departmentId && t.CreatedAt >= thirtyDaysAgo)
            };
        }

        public async Task<bool> NameExistsAsync(string name) => 
            await _context.Departments.AnyAsync(d => d.Name.ToLower() == name.ToLower());

        public async void Update(Department department) => _context.Departments.Update(department);
    }
}
