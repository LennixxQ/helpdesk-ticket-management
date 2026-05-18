using HelpDesk.API.Records;
using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.EscalationCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Priority;
using HelpDesk.Application.DTOs.Sla;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : BaseController
    {
        private readonly ITicketService _ticketService;
        private readonly IEscalationService _escalationService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, IEscalationService escalationService, IEmailTemplateService templateService, ILogger<TicketsController> logger, ICurrentUserProvider currentUserProvider):base(currentUserProvider)
        {
            _ticketService = ticketService;
            _templateService = templateService;
            _logger = logger;
            _escalationService = escalationService;
        }

        [HttpPost("createTicket")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
        {
            _logger.LogInformation("User {UserId} creating ticket with title: {Title}", _currentUserProvider, command.Title);
            var response = await _ticketService.CreateAsync(command, _currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("GetAllTicket")]
        public async Task<IActionResult> GetAll([FromBody] PaginationDto dto)
        {
            _logger.LogInformation("User {UserId} [{Role}] fetching tickets", _currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            var result = await _ticketService.GetAllAsync(dto,_currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return Ok(result);
        }

        [HttpPost("getByIdTicket")]
        public async Task<IActionResult> GetById([FromBody] GetByIdRequest dto)
        {
            var result = await _ticketService.GetByIdAsync(dto.Id,_currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("Agent-assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign([FromBody] AssignTicketCommand assignTicketCommand)
        {
            _logger.LogInformation("Admin {AdminId} assigning ticket {TicketId} to agent {AgentId}",_currentUserProvider.GetCurrentUserId(), assignTicketCommand.TicketId, assignTicketCommand.AgentId);
            var result = await _ticketService.AssignAsync(assignTicketCommand);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("UpdateTicketStatus")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTicketStatusCommand updateTicketStatus)
        {
            _logger.LogInformation("User {UserId} updating ticket {TicketId} status to {Status}",_currentUserProvider.GetCurrentUserId(), updateTicketStatus.TicketId, updateTicketStatus.NewStatus);
            var result = await _ticketService.UpdateStatusAsync(updateTicketStatus,_currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("UpdatePriority")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePriority([FromBody] UpdatePriorityRequest updatePriorityDto)
        {
            _logger.LogInformation("Admin {AdminId} updating priority of ticket {TicketId} to {Priority}",_currentUserProvider.GetCurrentUserId(), updatePriorityDto.TicketId, updatePriorityDto.Priority);
            var result = await _ticketService.UpdatePriorityAsync(updatePriorityDto.TicketId, updatePriorityDto.Priority);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("CloseTicket")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Close([FromBody] GetByIdRequest ticketIdDto)
        {
            _logger.LogInformation("Admin {AdminId} closing ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), ticketIdDto.Id);
            var result = await _ticketService.CloseAsync(ticketIdDto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Ticket-reopen")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reopen([FromBody] GetByIdRequest ticketIdDto)
        {
            _logger.LogInformation("Admin {AdminId} reopening ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), ticketIdDto.Id);
            var result = await _ticketService.ReopenAsync(ticketIdDto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Add-Comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand addComment)
        {
            var result = await _ticketService.AddCommentAsync(addComment, _currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("escalate")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Escalate([FromBody] EscalateRequest dto)
        {
            _logger.LogInformation("User {UserId} escalating ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), dto.TicketId);
            var result = await _escalationService.EscalateAsync(new EscalateTicketCommand
                {
                    TicketId = dto.TicketId,
                    Reason = dto.Reason
                },_currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("acknowledgeEscalation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcknowledgeEscalation([FromBody] GetByIdRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} acknowledging escalation for ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), dto.Id);
            var result = await _escalationService.AcknowledgeAsync(dto.Id, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("overrideSla")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OverrideSla([FromBody] SlaOverrideRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} overriding SLA for ticket {TicketId}",_currentUserProvider.GetCurrentUserId(), dto.TicketId);
            var result = await _ticketService.OverrideSlaAsync(dto, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST api/tickets/markResolvedViaKb
        [HttpPost("markResolvedViaKb")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> MarkResolvedViaKb([FromBody] ResolvedViaKbRequest dto)
        {
            var result = await _ticketService.MarkResolvedViaKbAsync(dto.TicketId, dto.ArticleId, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST api/tickets/getArchived
        [HttpPost("getArchived")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetArchived([FromBody] PaginationDto dto) =>
            Ok(await _ticketService.GetArchivedAsync(dto));

        [HttpGet("/csat/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Csat(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id, Guid.Empty, Domain.Enums.UserRole.Admin); // Bypass for survey
            if (!ticket.Success) return NotFound("Ticket not found.");

            var html = await _templateService.RenderCsatSurveyViewAsync(ticket.Data!);
            return Content(html, "text/html");
        }

        [HttpPost("submit-survey")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitSurvey([FromBody] CsatSubmission submission)
        {
            _logger.LogInformation("Submitting CSAT survey for ticket {TicketId}", submission.TicketId);
            var result = await _ticketService.SubmitCsatAsync(submission.TicketId, submission.Rating, submission.Comments);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("view/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> View(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id, Guid.Empty, Domain.Enums.UserRole.Admin); // Bypass for test view
            if (!ticket.Success) return NotFound("Ticket not found.");

            var html = await _templateService.RenderTicketViewAsync(ticket.Data!);
            return Content(html, "text/html");
        }
    }
}
