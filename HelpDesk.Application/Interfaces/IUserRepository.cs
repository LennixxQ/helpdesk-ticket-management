using HelpDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<User>> GetAgentsAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        void Update(User user);
    }
}
