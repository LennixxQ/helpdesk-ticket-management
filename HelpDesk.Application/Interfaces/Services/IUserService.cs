using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
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
    }
}
