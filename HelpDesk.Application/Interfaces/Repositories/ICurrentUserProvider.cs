using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUserId();
        string GetCurrentUserEmail();
        string GetCurrentUserName();
        UserRole GetCurrentUserRole();
        bool IsAuthenticated();
    }
}
