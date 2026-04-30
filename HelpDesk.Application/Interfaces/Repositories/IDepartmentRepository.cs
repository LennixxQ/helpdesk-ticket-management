using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByIdAsync(Guid id);
        Task<IEnumerable<Department>> GetActiveAsync();
        Task<DepartmentSummary> GetSummaryAsync(Guid departmentId);
    }
}
