using HelpDesk.API.Records;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/notification-preferences")]
    [ApiController]
    public class NotificationPreferencesController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NotificationPreferencesController> _logger;

        public NotificationPreferencesController(IUnitOfWork uow,ICurrentUserProvider currentUser,ILogger<NotificationPreferencesController> logger) : base(currentUser)
        {
            _uow = uow;
            _logger = logger;
        }

        // GET api/notification-preferences/getMine
        [HttpGet("getMine")]
        public async Task<IActionResult> GetMine()
        {
            var prefs = await _uow.NotificationPreferences.GetByIdAsync(_currentUserProvider.GetCurrentUserId());
            return Ok(new { success = true, data = prefs });
        }

        // POST api/notification-preferences/upsert
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] UpsertPreferenceRequest dto)
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            var role = _currentUserProvider.GetCurrentUserRole();

            // PRD 5.3 — Admin cannot disable SLA breach + escalation alerts
            if (role == UserRole.Admin && (dto.EventType == NotificationEventType.SlaBreached || dto.EventType == NotificationEventType.TicketEscalated) && !dto.IsEnabled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Admin cannot disable SLA breach and escalation notifications."
                });
            }

            _logger.LogInformation("User {UserId} updating notification preference {EventType} → {IsEnabled}",userId, dto.EventType, dto.IsEnabled);

            await _uow.NotificationPreferences.UpsertAsync(userId, dto.EventType, dto.IsEnabled);
            await _uow.SaveChangesAsync();

            return Ok(new { success = true, message = "Preference updated." });
        }
    }
}
