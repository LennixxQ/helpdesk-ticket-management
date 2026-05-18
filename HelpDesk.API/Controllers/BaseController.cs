using HelpDesk.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public abstract class BaseController : Controller
    {
        protected readonly ICurrentUserProvider _currentUserProvider;

        protected BaseController(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();

            // Controllers that don't require MFA completion
            var noMfaRequiredControllers = new[] { "Mfa", "Reports", "Dashboard", "KnowledgeBase", "Tickets" };

            // 1. MFA Check - Skip for read-only data controllers
            if (User.HasClaim("mfa_pending", "true") && !noMfaRequiredControllers.Contains(controllerName ?? ""))
            {
                context.Result = new ObjectResult(new {
                    success = false,
                    message = "Multi-Factor Authentication required. Access denied."
                }) { StatusCode = 403 };
                return;
            }

            // 2. Security Stamp Check (Invaliding tokens on password change)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var tokenStamp = User.FindFirst("SecurityStamp")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && !string.IsNullOrEmpty(tokenStamp))
            {
                var uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var user = await uow.Users.GetByIdAsync(Guid.Parse(userIdClaim));

                if (user == null || user.SecurityStamp != tokenStamp)
                {
                    context.Result = new UnauthorizedObjectResult(new { 
                        success = false, 
                        message = "Session expired or password changed. Please login again." 
                    });
                    return;
                }
            }

            await next();
        }
    }
}
