using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Department;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<BaseResponse<List<DepartmentDto>>> GetAllAsync();
        Task<BaseResponse<DepartmentDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<DepartmentSummaryDto>> GetSummaryAsync(Guid id);
        Task<BaseResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
        Task<BaseResponse<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentDto dto);
        Task<BaseResponse<object>> DeactivateAsync(Guid id);
        Task<BaseResponse<object>> AssignHeadAsync(Guid departmentId, Guid userId);
    }
}
