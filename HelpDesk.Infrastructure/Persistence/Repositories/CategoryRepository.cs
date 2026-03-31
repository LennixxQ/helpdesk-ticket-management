using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<Category?> GetByNameAsync(string name) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync() =>
            await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
    }
}
