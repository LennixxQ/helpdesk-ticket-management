using HelpDesk.API.Records;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController : BaseController
    {
        private readonly ISystemSettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SystemSettingsController> _logger;

        public SystemSettingsController(ISystemSettingService settingService,INotificationService notificationService,ICurrentUserProvider currentUser,ILogger<SystemSettingsController> logger) : base(currentUser)
        {
            _settingService = settingService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("getAll")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> GetAll() => 
            Ok(await _settingService.GetAllAsync());

        [HttpPost("getByKey")]
        public async Task<IActionResult> GetByKey([FromBody] GetByKeyRequest dto)
        {
            var result = await _settingService.GetByKeyAsync(dto.Key);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateSettingRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} updating setting '{Key}'",_currentUserProvider.GetCurrentUserId(), dto.Key);
            var result = await _settingService.UpdateAsync(dto.Key, dto.Value, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("sendTestEmail")]
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest dto)
        {
            var adminId = _currentUserProvider.GetCurrentUserId();
            _logger.LogInformation("Admin {AdminId} sending test email to {Email}", adminId, dto.ToEmail);
            await _notificationService.SendTestEmailAsync(adminId, dto.ToEmail);
            return Ok(new { success = true, message = "Test email sent successfully." });
        }
    }
}
