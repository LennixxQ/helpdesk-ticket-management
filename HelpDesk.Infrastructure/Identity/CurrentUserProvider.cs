using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HelpDesk.Infrastructure.Identity
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        string ICurrentUserProvider.GetCurrentUserEmail()
        {
            throw new NotImplementedException();
        }

        public Guid GetCurrentUserId()
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier) ?? User?.FindFirst("sub");
            if (claim is null || !Guid.TryParse(claim.Value, out var id))
                return Guid.Empty;
            return id;
        }

        public string GetCurrentUserName()
        {
            var email = User?.FindFirst(ClaimTypes.Email)?.Value ?? User?.FindFirst("unique_name")?.Value ?? User?.FindFirst("name")?.Value ?? "admin@helpdesk.com";
            return email;
        }

        public UserRole GetCurrentUserRole()
        {
            var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value ?? User?.FindFirst("role")?.Value;
            if (string.IsNullOrEmpty(roleClaim))
                return UserRole.User;
            var role = Enum.TryParse<UserRole>(roleClaim, out var userRole) ? userRole : UserRole.User;
            return role;
        }

        public bool IsAuthenticated()
        {
            var authenticated = User?.Identity?.IsAuthenticated ?? false;
            return authenticated;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    }
}
