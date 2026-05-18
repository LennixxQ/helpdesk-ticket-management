using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<IEnumerable<Department>> GetActiveAsync();
        Task<IEnumerable<Department>> GetAllWithIncludesAsync();
        Task<(int ActiveUsers, int OpenTickets, int Last30Days)> GetSummaryAsync(Guid departmentId);
    }
}
