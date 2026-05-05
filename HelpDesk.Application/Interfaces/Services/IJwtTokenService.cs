using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateMfaToken(User user);
    }
}
