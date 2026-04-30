using HelpDesk.Application.Commands.AuthCommand;
using HelpDesk.Application.Common;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> LoginAsync(LoginRequest request);
    }
}
