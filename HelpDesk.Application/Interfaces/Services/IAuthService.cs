using HelpDesk.Application.Commands.AuthCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Auth;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
