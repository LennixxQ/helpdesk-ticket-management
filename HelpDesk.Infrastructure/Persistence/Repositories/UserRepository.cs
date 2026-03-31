using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role) =>
            await _context.Users.Where(u => u.Role == role).ToListAsync();

        public async Task<IEnumerable<User>> GetActiveAgentsAsync() =>
            await _context.Users.Where(u => u.Role == UserRole.Agent && u.IsActive).OrderBy(u => u.FullName).ToListAsync();
    }
}
