using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IUnitOfWork uow, ILogger<NotificationsController> logger, ICurrentUserProvider currentUserProvider) : base(currentUserProvider)
        {
            _uow = uow;
            _logger = logger;
        }

        public record NotificationItem(
            string Id,
            string Title,
            string Message,
            string Type, // "Info", "Warning", "Error", "Success"
            DateTime CreatedAt,
            string RouteUrl,
            bool IsRead
        );

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = _currentUserProvider.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(BaseResponse<List<NotificationItem>>.Fail("User is not authenticated."));
                }

                var user = await _uow.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(BaseResponse<List<NotificationItem>>.Fail("User not found."));
                }

                var notifications = new List<NotificationItem>();
                var role = user.Role;

                if (role == UserRole.Admin || role == UserRole.DepartmentHead)
                {
                    // 1. Fetch unassigned tickets (created in the last 7 days)
                    var allTickets = await _uow.Tickets.GetAllAsync();
                    var unassigned = allTickets
                        .Where(t => t.AssignedAgentId == null && t.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                        .OrderByDescending(t => t.CreatedAt);

                    foreach (var ticket in unassigned)
                    {
                        notifications.Add(new NotificationItem(
                            $"ticket-new-{ticket.Id}",
                            "New Unassigned Ticket",
                            $"Ticket #{ticket.Id.ToString().Substring(0, 8)}: '{ticket.Title}' is pending assignment.",
                            "Info",
                            ticket.CreatedAt,
                            $"/tickets/{ticket.Id}",
                            false
                        ));
                    }

                    // 2. Fetch escalated tickets
                    var escalated = allTickets
                        .Where(t => t.Status != TicketStatus.Closed && t.IsEscalated)
                        .OrderByDescending(t => t.CreatedAt);

                    foreach (var ticket in escalated)
                    {
                        notifications.Add(new NotificationItem(
                            $"ticket-esc-{ticket.Id}",
                            "Escalated Ticket Alert",
                            $"Ticket #{ticket.Id.ToString().Substring(0, 8)}: '{ticket.Title}' has been escalated!",
                            "Error",
                            ticket.LastModifiedAt ?? ticket.CreatedAt,
                            $"/tickets/{ticket.Id}",
                            false
                        ));
                    }
                }
                
                if (role == UserRole.Agent)
                {
                    // 1. Fetch tickets assigned to this agent
                    var assignedTickets = await _uow.Tickets.GetByAgentIdAsync(userId);
                    var activeAssigned = assignedTickets
                        .Where(t => t.Status != TicketStatus.Closed)
                        .OrderByDescending(t => t.CreatedAt);

                    foreach (var ticket in activeAssigned)
                    {
                        notifications.Add(new NotificationItem(
                            $"ticket-assign-{ticket.Id}",
                            "New Assignment",
                            $"You have been assigned Ticket #{ticket.Id.ToString().Substring(0, 8)}: '{ticket.Title}'",
                            "Success",
                            ticket.LastModifiedAt ?? ticket.CreatedAt,
                            $"/tickets/{ticket.Id}",
                            false
                        ));

                        // 2. High priority alert
                        if (ticket.Priority == TicketPriority.High || ticket.Priority == TicketPriority.Critical)
                        {
                            notifications.Add(new NotificationItem(
                                $"ticket-pri-{ticket.Id}",
                                "High Priority Ticket",
                                $"Assigned Ticket #{ticket.Id.ToString().Substring(0, 8)} has high/critical priority!",
                                "Warning",
                                ticket.CreatedAt,
                                $"/tickets/{ticket.Id}",
                                false
                            ));
                        }
                    }

                    // 3. Comments on their assigned tickets (posted by other users)
                    var comments = await _uow.Comments.GetAllAsync();
                    var activeAssignedIds = activeAssigned.Select(t => t.Id).ToHashSet();
                    var customerComments = comments
                        .Where(c => activeAssignedIds.Contains(c.TicketId) && c.CreatedBy != userId.ToString() && c.CreatedAt >= DateTime.UtcNow.AddDays(-3))
                        .OrderByDescending(c => c.CreatedAt);

                    foreach (var comment in customerComments)
                    {
                        notifications.Add(new NotificationItem(
                            $"comment-new-{comment.Id}",
                            "Client Response",
                            $"New comment added on assigned Ticket #{comment.TicketId.ToString().Substring(0, 8)}...",
                            "Info",
                            comment.CreatedAt,
                            $"/tickets/{comment.TicketId}",
                            false
                        ));
                    }
                }

                if (role == UserRole.User)
                {
                    // 1. Fetch tickets raised by this user
                    var raisedTickets = await _uow.Tickets.GetByUserIdAsync(userId);
                    
                    foreach (var ticket in raisedTickets.OrderByDescending(t => t.LastModifiedAt ?? t.CreatedAt).Take(20))
                    {
                        // Active status updates in the last 7 days
                        if (ticket.Status != TicketStatus.Open && ticket.LastModifiedAt >= DateTime.UtcNow.AddDays(-7))
                        {
                            notifications.Add(new NotificationItem(
                                $"ticket-status-{ticket.Id}",
                                "Ticket Status Updated",
                                $"Your Ticket #{ticket.Id.ToString().Substring(0, 8)} status is now: {ticket.Status}",
                                "Success",
                                ticket.LastModifiedAt ?? ticket.CreatedAt,
                                $"/tickets/{ticket.Id}",
                                false
                            ));
                        }
                    }

                    // 2. Fetch comments on their tickets from agents
                    var comments = await _uow.Comments.GetAllAsync();
                    var raisedTicketIds = raisedTickets.Select(t => t.Id).ToHashSet();
                    var agentComments = comments
                        .Where(c => raisedTicketIds.Contains(c.TicketId) && c.CreatedBy != userId.ToString() && c.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                        .OrderByDescending(c => c.CreatedAt);

                    foreach (var comment in agentComments)
                    {
                        notifications.Add(new NotificationItem(
                            $"comment-agent-{comment.Id}",
                            "Agent Replied",
                            $"An agent has added a reply to your Ticket.",
                            "Info",
                            comment.CreatedAt,
                            $"/tickets/{comment.TicketId}",
                            false
                        ));
                    }
                }

                // Order notifications by creation date descending
                var result = notifications.OrderByDescending(n => n.CreatedAt).ToList();

                return Ok(BaseResponse<List<NotificationItem>>.Ok(result, "Notifications retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role-wise notifications.");
                return StatusCode(500, BaseResponse<List<NotificationItem>>.Fail("An error occurred while fetching notifications."));
            }
        }
    }
}
