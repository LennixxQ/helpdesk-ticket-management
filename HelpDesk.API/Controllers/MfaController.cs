using HelpDesk.Application.DTOs.MFA;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfaController : BaseController
    {
        private readonly IMfaService _mfaService;
        private readonly ILogger<MfaController> _logger;

        public MfaController(IMfaService mfaService, ILogger<MfaController> logger, ICurrentUserProvider currentUserProvider) 
            : base(currentUserProvider)
        {
            _mfaService = mfaService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("setup")]
        public async Task<IActionResult> GetSetup()
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            _logger.LogInformation("Generating MFA setup for user: {UserId}", userId);
            
            var response = await _mfaService.GetMfaSetupAsync(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("verify-setup")]
        public async Task<IActionResult> VerifySetup([FromBody] VerifyMfaRequest request)
        {
            Guid? userId = null;
            if (string.IsNullOrEmpty(request.JwtToken))
            {
                if (!_currentUserProvider.IsAuthenticated())
                    return Unauthorized("Authentication required or JwtToken missing.");
                
                userId = _currentUserProvider.GetCurrentUserId();
            }

            _logger.LogInformation("Verifying MFA setup.");

            var response = await _mfaService.VerifyAndEnableMfaAsync(request.JwtToken, request.Code, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("verify-login")]
        public async Task<IActionResult> VerifyLogin([FromBody] VerifyMfaRequest request)
        {
            if (string.IsNullOrEmpty(request.JwtToken))
                return BadRequest("JwtToken is required.");

            _logger.LogInformation("Verifying MFA login code.");

            var response = await _mfaService.VerifyLoginCodeAsync(request.JwtToken, request.Code);
            return response.Success ? Ok(response) : Unauthorized(response);
        }
        [Authorize]
        [HttpPost("disable")]
        public async Task<IActionResult> Disable([FromBody] VerifyMfaRequest request)
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            _logger.LogInformation("Disabling MFA for user: {UserId}", userId);

            var response = await _mfaService.DisableMfaAsync(userId, request.Code);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
