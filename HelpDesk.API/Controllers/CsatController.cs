using HelpDesk.API.Records;
using HelpDesk.Application.Commands.CsatCommand;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsatController : BaseController
    {
        private readonly ICsatService _csatService;
        private readonly ILogger<CsatController> _logger;

        public CsatController(ICsatService csatService,ICurrentUserProvider currentUser,ILogger<CsatController> logger) : base(currentUser)
        {
            _csatService = csatService;
            _logger = logger;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Submit([FromBody] SubmitCsatCommand command)
        {
            _logger.LogInformation("User {UserId} submitting CSAT for ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), command.TicketId);
            var result = await _csatService.SubmitAsync(command, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("getAgentStats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAgentStats([FromBody] AgentStatsRequest dto) =>
            Ok(await _csatService.GetAgentStatsAsync(dto.AgentId, dto.From, dto.To));
    }
}
