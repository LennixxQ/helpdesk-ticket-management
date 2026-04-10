using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
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
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger, ICurrentUserProvider currentUserProvider):base(currentUserProvider)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpPost("createTicket")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
        {
            _logger.LogInformation("User {UserId} creating ticket with title: {Title}",CurrentUserId, command.Title);
            var response = await _ticketService.CreateAsync(command, CurrentUserId, CurrentUserRole);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("GetAllTicket")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto dto)
        {
            var response = await _ticketService.GetAllAsync(dto, CurrentUserId, CurrentUserRole);
            return Ok(response);
        }

        [HttpGet("getByIdTicket")]
        public async Task<IActionResult> GetById([FromQuery] Guid ticketId)
        {
            var response = await _ticketService.GetByIdAsync(ticketId, CurrentUserId, CurrentUserRole);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("Agent-assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign([FromBody] AssignTicketCommand assignTicketCommand)
        {
            var response = await _ticketService.AssignAsync(assignTicketCommand);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("UpdateTicketStatus")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTicketStatusCommand updateTicketStatus)
        {
            var updateStatus = await _ticketService.UpdateStatusAsync(updateTicketStatus, CurrentUserId, CurrentUserRole);
            return updateStatus.Success ? Ok(updateStatus) : BadRequest(updateStatus);
        }


        [HttpPut("UpdatePriority")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePriority([FromBody] UpdatePriorityDto updatePriorityDto)
        {
            var updatePriority = await _ticketService.UpdatePriorityAsync(updatePriorityDto.TicketId, updatePriorityDto.Priority);
            return updatePriority.Success ? Ok(updatePriority) : BadRequest(updatePriority);
        }

        [HttpPut("CloseTicket")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Close([FromBody] TicketIdDto ticketIdDto)
        {
            var closeTicket = await _ticketService.CloseAsync(ticketIdDto.TicketId);
            return closeTicket.Success ? Ok(closeTicket) : BadRequest(closeTicket);
        }

        [HttpPut("Ticket-reopen")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reopen([FromBody] TicketIdDto ticketIdDto)
        {
            var reopen = await _ticketService.ReopenAsync(ticketIdDto.TicketId);
            return reopen.Success ? Ok(reopen) : BadRequest(reopen);
        }

        [HttpPost("Add-Comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand addComment)
        {
            var response = await _ticketService.AddCommentAsync(addComment, CurrentUserId, CurrentUserRole);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        public record UpdatePriorityDto(Guid TicketId, TicketPriority Priority);
        public record TicketIdDto(Guid TicketId);
    }
}
