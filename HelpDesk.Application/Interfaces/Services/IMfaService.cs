using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Auth;
using HelpDesk.Application.DTOs.MFA;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IMfaService
    {
        Task<BaseResponse<MfaSetupDto>> GetMfaSetupAsync(Guid userId);
        Task<BaseResponse<bool>> VerifyAndEnableMfaAsync(string? jwtToken, string code, Guid? userId = null);
        Task<BaseResponse<bool>> DisableMfaAsync(Guid userId, string code);
        Task<BaseResponse<LoginResponse>> VerifyLoginCodeAsync(string jwtToken, string code);
        string GenerateMfaSessionToken(Guid userId);
    }
}
