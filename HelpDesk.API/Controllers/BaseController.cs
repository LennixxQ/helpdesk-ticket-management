using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ICurrentUserProvider _currentUserProvider;

        protected BaseController(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        protected Guid CurrentUserId => _currentUserProvider.GetCurrentUserId();
        protected UserRole CurrentUserRole => _currentUserProvider.GetCurrentUserRole();
        protected string CurrentUserName => _currentUserProvider.GetCurrentUserName();
        protected string CurrentUserEmail => _currentUserProvider.GetCurrentUserEmail();
        protected bool IsAuthenticated => _currentUserProvider.IsAuthenticated();

    }
}
