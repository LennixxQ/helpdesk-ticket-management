using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginRequest.Email);
            var response = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        public record LoginRequest(string Email, string Password);
    }
}
