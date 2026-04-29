using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<IEnumerable<Department>> GetActiveAsync();
        Task AddAsync(Department department);
        void Update(Department department);
        Task<bool> NameExistsAsync(string name);
        Task<DepartmentSummary> GetSummaryAsync(Guid departmentId);
    }
}
