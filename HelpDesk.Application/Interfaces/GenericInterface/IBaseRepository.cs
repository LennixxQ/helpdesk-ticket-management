using HelpDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces.GenericInterface
{
    public interface IBaseRepository<T> where T:BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}
