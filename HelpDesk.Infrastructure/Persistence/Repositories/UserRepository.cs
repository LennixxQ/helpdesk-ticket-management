using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using System.Data.Entity;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.NormalizedEmail == email.ToUpper(), ct);
            return emailExists;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            var result = await _context.Users.AnyAsync(u => u.Id == id, ct);
            return result;
        }

        public async Task<IEnumerable<User>> GetAgentsAsync(CancellationToken ct = default)
        {
            var agent = await _context.Users.Where(u => u.Role == UserRole.SupportAgent && u.IsActive).ToListAsync(ct);
            return agent;
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        {
            var getAllAgent = await _context.Users.ToListAsync(ct);
            return getAllAgent;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToUpper(), ct);
            return user;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var userById = await _context.Users.FindAsync([id], ct);
            return userById;
        }

        public void Update(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
        }
    }
}
