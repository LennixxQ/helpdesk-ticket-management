using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.DeletedAt = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
