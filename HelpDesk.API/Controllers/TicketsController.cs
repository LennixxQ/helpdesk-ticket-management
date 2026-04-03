using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private UserRole CurrentUserRole => Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

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
            var response = await _ticketService.GetAllAsync(dto.page, dto.pageSize, dto.status, dto.priority, dto.categoryId, dto.agentId, CurrentUserId, CurrentUserRole);
            return Ok(response);
        }

        [HttpGet("getByIdTicket")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _ticketService.GetByIdAsync(id, CurrentUserId, CurrentUserRole);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("Agent-assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(Guid id, [FromBody] AssignAgentRequest request)
        {
            var command = new AssignTicketCommand { TicketId = id, AgentId = request.AgentId };
            var response = await _ticketService.AssignAsync(command);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("UpdateTicketStatus")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var command = new UpdateTicketStatusCommand
            {
                TicketId = id,
                NewStatus = request.NewStatus
            };

            var updateStatus = await _ticketService.UpdateStatusAsync(command, CurrentUserId, CurrentUserRole);
            return updateStatus.Success ? Ok(updateStatus) : BadRequest(updateStatus);
        }


        [HttpPut("UpdatePriority")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePriority(Guid id, [FromBody] UpdatePriorityRequest request)
        {
            var updatePriority = await _ticketService.UpdatePriorityAsync(id, request.Priority);
            return updatePriority.Success ? Ok(updatePriority) : BadRequest(updatePriority);
        }

        [HttpPut("CloseTicket")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Close(Guid id)
        {
            var closeTicket = await _ticketService.CloseAsync(id);
            return closeTicket.Success ? Ok(closeTicket) : BadRequest(closeTicket);
        }

        [HttpPut("Ticket-reopen")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reopen(Guid id)
        {
            var reopen = await _ticketService.ReopenAsync(id);
            return reopen.Success ? Ok(reopen) : BadRequest(reopen);
        }

        [HttpPost("Add-Comment")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentRequest request)
        {
            var command = new AddCommentCommand
            {
                TicketId = id,
                Content = request.Content
            };

            var response = await _ticketService.AddCommentAsync(
                command, CurrentUserId, CurrentUserRole);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        public record AssignAgentRequest(Guid AgentId);
        public record UpdateStatusRequest(TicketStatus NewStatus);
        public record UpdatePriorityRequest(TicketPriority Priority);
        public record AddCommentRequest(string Content);
    }
}
