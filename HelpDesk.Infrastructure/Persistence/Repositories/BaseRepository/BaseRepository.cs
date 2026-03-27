using HelpDesk.Application.Interfaces.GenericInterface;
using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T: BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _dbSet.FindAsync([id], ct);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) => await _dbSet.ToListAsync(ct);

        public virtual async Task AddAsync(T entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public virtual void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => await _dbSet.AnyAsync(e => e.Id == id, ct);
    }
}
