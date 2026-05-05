using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Import;
using HelpDesk.Application.DTOs.User;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserDto>> CreateUserAsync(CreateUserCommand command);
        Task<BaseResponse<List<UserDto>>> GetAllUsersAsync();
        Task<BaseResponse<UserDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<UserDto>> UpdateRoleAsync(UpdateUserRoleCommand command);
        Task<BaseResponse<UserDto>> DeactivateAsync(Guid userId);
        Task<BaseResponse<List<UserDto>>> GetActiveAgentsAsync();
        Task<BaseResponse<object>> MoveDepartmentAsync(Guid userId, Guid departmentId);
        Task<BaseResponse<BulkImportResultDto>> BulkImportAsync(List<BulkImportRowDto> rows, Guid adminId);
    }
}
