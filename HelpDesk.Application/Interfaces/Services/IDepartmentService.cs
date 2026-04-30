using HelpDesk.Application.Commands.DepartmentCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Department;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<BaseResponse<List<DepartmentDto>>> GetAllAsync();
        Task<BaseResponse<DepartmentDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<DepartmentSummaryDto>> GetSummaryAsync(Guid id);
        Task<BaseResponse<DepartmentDto>> CreateAsync(CreateDepartmentCommand command);
        Task<BaseResponse<DepartmentDto>> UpdateAsync(UpdateDepartmentCommand command);
        Task<BaseResponse<object>> DeactivateAsync(Guid id);
        Task<BaseResponse<object>> AssignHeadAsync(Guid departmentId, Guid userId);
    }
}
