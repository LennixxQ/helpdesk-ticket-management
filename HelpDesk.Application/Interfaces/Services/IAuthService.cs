using HelpDesk.Application.Common;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> LoginAsync(string email, string password);
    }
}
