using AutoMapper;
using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Comment;
using HelpDesk.Application.DTOs.Sla;
using HelpDesk.Application.DTOs.Ticket;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ISlaDeadlineCalculator _slaCalculator;
        private readonly CreateTicketValidator _createValidator;
        private readonly AssignTicketValidator _assignValidator;
        private readonly AddCommentValidator _commentValidator;
        private readonly INotificationService _notificationService;
        private readonly SlaOverrideValidator _slaOverrideValidator;

        public TicketService(IUnitOfWork uow,IMapper mapper,ISlaDeadlineCalculator slaCalculator, INotificationService notificationService)
        {
            _uow = uow;
            _mapper = mapper;
            _slaCalculator = slaCalculator;
            _notificationService = notificationService;
            _createValidator = new CreateTicketValidator();
            _assignValidator = new AssignTicketValidator();
            _commentValidator = new AddCommentValidator();
            _slaOverrideValidator = new SlaOverrideValidator();
        }

        public async Task<BaseResponse<CreateTicketResponseDto>> CreateAsync(
            CreateTicketCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var validation = await _createValidator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<CreateTicketResponseDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var category = await _uow.Categories.GetByIdAsync(command.CategoryId);
            if (category is null || !category.IsActive)
                return BaseResponse<CreateTicketResponseDto>.Fail("Invalid or inactive category.");

            var raisedByUserId = command.RaisedByUserId ?? currentUserId;

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                CategoryId = command.CategoryId,
                Priority = command.Priority,
                Status = TicketStatus.Open,
                RaisedByUserId = raisedByUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };

            await _uow.Tickets.AddAsync(ticket);
            await _uow.SaveChangesAsync();

            // SLA record — business hours aware
            var policy = await _uow.Sla.GetPolicyByPriorityAsync(command.Priority);
            if (policy is not null)
            {
                var deadline = _slaCalculator.Calculate(DateTime.UtcNow, policy.ResolutionMinutes);
                var slaRecord = new SlaRecord
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    SlaDeadline = deadline,
                    Status = SlaStatus.WithinSla,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId.ToString()
                };
                await _uow.Sla.AddAsync(slaRecord);
                ticket.SlaDeadline = deadline;
                _uow.Tickets.Update(ticket);
                await _uow.SaveChangesAsync();
            }

            // Notify parties (PRD 5.2)
            await _notificationService.SendTicketCreatedAsync(ticket);

            return BaseResponse<CreateTicketResponseDto>.Ok(
                _mapper.Map<CreateTicketResponseDto>(ticket), "Ticket created.");
        }

        public async Task<BaseResponse<TicketDto>> AssignAsync(AssignTicketCommand command)
        {
            var validation = await _assignValidator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<TicketDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            var agent = await _uow.Users.GetByIdAsync(command.AgentId);
            if (agent is null || !agent.IsActive)
                return BaseResponse<TicketDto>.Fail("Agent not found or inactive.");

            if (agent.Role != UserRole.Agent)
                return BaseResponse<TicketDto>.Fail("Only agents can be assigned to tickets.");

            if (ticket.Status == TicketStatus.Closed)
                return BaseResponse<TicketDto>.Fail("Cannot assign a closed ticket.");

            ticket.AssignedAgentId = command.AgentId;
            ticket.Status = TicketStatus.InProgress;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);

            // Notify parties (PRD 5.2)
            if (updated != null)
            {
                await _notificationService.SendTicketAssignedAsync(updated);
            }

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Ticket assigned.");
        }

        public async Task<BaseResponse<TicketDto>> UpdateStatusAsync(
            UpdateTicketStatusCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            var oldStatus = ticket.Status;

            if (currentUserRole == UserRole.Agent && ticket.AssignedAgentId != currentUserId)
                return BaseResponse<TicketDto>.Fail("You can only update tickets assigned to you.");

            var allowed = new Dictionary<TicketStatus, TicketStatus[]>
            {
                [TicketStatus.Open] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
                [TicketStatus.InProgress] = new[] { TicketStatus.OnHold, TicketStatus.Resolved },
                [TicketStatus.OnHold] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
                [TicketStatus.Resolved] = new[] { TicketStatus.Closed, TicketStatus.Reopened },
                [TicketStatus.Closed] = new[] { TicketStatus.Reopened },
                [TicketStatus.Reopened] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
            };

            if (!allowed.TryGetValue(ticket.Status, out var validNext) ||
                !validNext.Contains(command.NewStatus))
                return BaseResponse<TicketDto>.Fail(
                    $"Cannot transition from '{ticket.Status}' to '{command.NewStatus}'.");

            if (command.NewStatus is TicketStatus.Closed or TicketStatus.Reopened &&
                currentUserRole != UserRole.Admin)
                return BaseResponse<TicketDto>.Fail($"Only Admin can set '{command.NewStatus}'.");

            // SLA pause/resume
            var slaRecord = await _uow.Sla.GetByTicketIdAsync(ticket.Id);
            if (slaRecord is not null)
            {
                if (command.NewStatus == TicketStatus.OnHold && slaRecord.PausedAt is null)
                {
                    slaRecord.PausedAt = DateTime.UtcNow;
                    _uow.Sla.Update(slaRecord);
                }
                else if (command.NewStatus == TicketStatus.InProgress && slaRecord.PausedAt is not null)
                {
                    slaRecord.TotalPausedMinutes += (int)(DateTime.UtcNow - slaRecord.PausedAt.Value).TotalMinutes;
                    slaRecord.PausedAt = null;
                    _uow.Sla.Update(slaRecord);
                }
            }

            if (command.NewStatus == TicketStatus.Reopened)
            {
                ticket.ReopenCount++;
                ticket.IsEscalated = false;
            }

            ticket.Status = command.NewStatus;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);

            // Notify parties (PRD 5.2)
            if (updated != null)
            {
                if (command.NewStatus == TicketStatus.Closed)
                {
                    await _notificationService.SendTicketClosedAsync(updated);
                }
                else
                {
                    await _notificationService.SendStatusChangedAsync(updated, oldStatus.ToString());
                }

                // If resolved or closed, send CSAT survey request (PRD 5.2 / 11.2)
                if (command.NewStatus is TicketStatus.Resolved or TicketStatus.Closed)
                {
                    await _notificationService.SendCsatSurveyAsync(updated);
                }
            }

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Status updated.");
        }

        public async Task<BaseResponse<CommentDto>> AddCommentAsync(
            AddCommentCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var validation = await _commentValidator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<CommentDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<CommentDto>.Fail("Ticket not found.");

            var user = await _uow.Users.GetByIdAsync(currentUserId);
            if (user is null) return BaseResponse<CommentDto>.Fail("User not found.");

            var isRaiser = ticket.RaisedByUserId == currentUserId;
            var isAgent = ticket.AssignedAgentId == currentUserId;
            var isAdmin = currentUserRole == UserRole.Admin;

            if (!isRaiser && !isAgent && !isAdmin)
                return BaseResponse<CommentDto>.Fail("Not authorized to comment on this ticket.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TicketId = command.TicketId,
                UserId = currentUserId,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };

            await _uow.Comments.AddAsync(comment);
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            comment.User = user;

            // Notify parties (PRD 5.2)
            await _notificationService.SendCommentAddedAsync(ticket, comment);

            return BaseResponse<CommentDto>.Ok(_mapper.Map<CommentDto>(comment), "Comment added.");
        }

        public async Task<BaseResponse<TicketDto>> GetByIdAsync(
            Guid id, Guid currentUserId, UserRole currentUserRole)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(id);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            var isAdmin = currentUserRole == UserRole.Admin;
            var isRaiser = ticket.RaisedByUserId == currentUserId;
            var isAgent = ticket.AssignedAgentId == currentUserId;

            if (!isAdmin && !isRaiser && !isAgent)
                return BaseResponse<TicketDto>.Fail("Access denied.");

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket));
        }

        public async Task<BaseResponse<PagedResult<TicketDto>>> GetAllAsync(
            PaginationDto dto, Guid currentUserId, UserRole currentUserRole)
        {
            Guid? raisedByFilter = currentUserRole == UserRole.User ? currentUserId : null;
            Guid? agentFilter = currentUserRole == UserRole.Agent ? currentUserId : null;

            var paged = await _uow.Tickets.GetAllPagedAsync(
                dto.Page, dto.PageSize, dto.Status, dto.Priority,
                dto.CategoryId, agentFilter, raisedByFilter);

            return BaseResponse<PagedResult<TicketDto>>.Ok(new PagedResult<TicketDto>
            {
                Items = _mapper.Map<List<TicketDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            });
        }

        public async Task<BaseResponse<TicketDto>> ReopenAsync(Guid ticketId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");
            if (ticket.Status != TicketStatus.Closed && ticket.Status != TicketStatus.Resolved)
                return BaseResponse<TicketDto>.Fail("Only Closed or Resolved tickets can be reopened.");

            ticket.Status = TicketStatus.Reopened;
            ticket.ReopenCount++;
            ticket.LastModifiedAt = DateTime.UtcNow;

            // PRD 10.2 — Auto-escalate on 3rd reopen
            if (ticket.ReopenCount >= 3 && !ticket.IsEscalated)
            {
                ticket.IsEscalated = true;
                ticket.Priority = ticket.Priority switch
                {
                    TicketPriority.Low => TicketPriority.Medium,
                    TicketPriority.Medium => TicketPriority.High,
                    TicketPriority.High => TicketPriority.Critical,
                    _ => ticket.Priority
                };
                ticket.EscalationRecord = new EscalationRecord
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    Reason = $"System Auto-Escalation: Ticket reopened {ticket.ReopenCount} times.",
                    Trigger = EscalationTrigger.UserReopenedThreeTimes,
                    EscalatedBy = "System",
                    EscalatedByUserId = null,
                    EscalatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                await _notificationService.SendTicketEscalatedAsync(ticket, ticket.EscalationRecord);
            }

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
            
            // Notify parties (PRD 5.2)
            if (updated != null)
            {
                await _notificationService.SendStatusChangedAsync(updated, "Reopened");
            }

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Ticket reopened.");
        }

        public async Task<BaseResponse<TicketDto>> CloseAsync(Guid ticketId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");
            if (ticket.Status == TicketStatus.Closed)
                return BaseResponse<TicketDto>.Fail("Ticket is already closed.");

            ticket.Status = TicketStatus.Closed;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);

            // Notify parties (PRD 5.2)
            if (updated != null)
            {
                await _notificationService.SendTicketClosedAsync(updated);
                await _notificationService.SendCsatSurveyAsync(updated);
            }

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Ticket closed.");
        }

        public async Task<BaseResponse<TicketDto>> UpdatePriorityAsync(Guid ticketId, TicketPriority priority)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            // PRD 6.3 — Recalculate SLA on priority change
            var policy = await _uow.Sla.GetPolicyByPriorityAsync(priority);
            var slaRecord = await _uow.Sla.GetByTicketIdAsync(ticketId);
            if (policy is not null && slaRecord is not null && !slaRecord.IsOverridden)
            {
                var newDeadline = _slaCalculator.Calculate(DateTime.UtcNow, policy.ResolutionMinutes);
                slaRecord.SlaDeadline = newDeadline;
                slaRecord.IsBreached = false;
                slaRecord.Status = SlaStatus.WithinSla;
                ticket.SlaDeadline = newDeadline;
                ticket.SlaBreached = false;
                ticket.SlaStatus = SlaStatus.WithinSla;
                _uow.Sla.Update(slaRecord);
            }

            ticket.Priority = priority;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Priority updated.");
        }

        public async Task<BaseResponse<object>> OverrideSlaAsync(
            SlaOverrideRequest request, Guid adminId)
        {
            var validation = await _slaOverrideValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BaseResponse<object>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(request.TicketId);
            if (ticket is null) return BaseResponse<object>.Fail("Ticket not found.");

            var slaRecord = await _uow.Sla.GetByTicketIdAsync(request.TicketId);
            if (slaRecord is null) return BaseResponse<object>.Fail("No SLA record found.");

            slaRecord.SlaDeadline = request.NewDeadline;
            slaRecord.IsOverridden = true;
            slaRecord.OverriddenById = adminId;
            slaRecord.OverrideReason = request.Reason;
            slaRecord.IsBreached = false;
            slaRecord.Status = SlaStatus.WithinSla;

            ticket.SlaDeadline = request.NewDeadline;
            ticket.SlaBreached = false;
            ticket.SlaStatus = SlaStatus.WithinSla;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Sla.Update(slaRecord);
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "SLA deadline overridden successfully.");
        }

        public async Task<BaseResponse<object>> MarkResolvedViaKbAsync(
            Guid ticketId, Guid articleId, Guid agentId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<object>.Fail("Ticket not found.");

            if (ticket.AssignedAgentId != agentId)
                return BaseResponse<object>.Fail("Only the assigned agent can mark resolved via KB.");

            var article = await _uow.KbArticles.GetByIdAsync(articleId);
            if (article is null) return BaseResponse<object>.Fail("KB article not found.");

            ticket.IsResolvedViaKb = true;
            ticket.ResolvedViaKbArticleId = articleId;
            ticket.Status = TicketStatus.Resolved;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Ticket marked as resolved via KB.");
        }

        public async Task<BaseResponse<object>> SubmitCsatAsync(Guid ticketId, int rating, string? comments)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<object>.Fail("Ticket not found.");

            if (await _uow.Csat.ExistsForTicketAsync(ticketId))
                return BaseResponse<object>.Fail("Survey already submitted for this ticket.");

            var csat = new CsatResponse
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                RespondentId = ticket.RaisedByUserId,
                ClosingAgentId = ticket.AssignedAgentId ?? Guid.Empty,
                Score = rating,
                Comments = comments,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            await _uow.Csat.AddAsync(csat);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Thank you for your feedback!");
        }

        public async Task<BaseResponse<PagedResult<TicketDto>>> GetArchivedAsync(PaginationDto dto)
        {
            var items = await _uow.TicketReports.GetArchivedPagedAsync(dto.Page, dto.PageSize);
            var total = await _uow.TicketReports.GetArchivedCountAsync();

            return BaseResponse<PagedResult<TicketDto>>.Ok(new PagedResult<TicketDto>
            {
                Items = _mapper.Map<List<TicketDto>>(items),
                TotalCount = total,
                Page = dto.Page,
                PageSize = dto.PageSize
            });
        }
    }
}