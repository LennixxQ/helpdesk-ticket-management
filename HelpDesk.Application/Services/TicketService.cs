using AutoMapper;
using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Comment;
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
        private readonly CreateTicketValidator _createValidator;
        private readonly AssignTicketValidator _assignValidator;
        private readonly AddCommentValidator _commentValidator;

        public TicketService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _createValidator = new CreateTicketValidator();
            _assignValidator = new AssignTicketValidator();
            _commentValidator = new AddCommentValidator();
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

            // Auto-create SLA record
            var policy = await _uow.Sla.GetPolicyByPriorityAsync(command.Priority);
            if (policy is not null)
            {
                var slaRecord = new SlaRecord
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    SlaDeadline = DateTime.UtcNow.AddMinutes(policy.ResolutionMinutes),
                    Status = SlaStatus.WithinSla,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId.ToString()
                };
                await _uow.Sla.AddAsync(slaRecord);
                ticket.SlaDeadline = slaRecord.SlaDeadline;
            }

            await _uow.SaveChangesAsync();

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
            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Ticket assigned.");
        }

        public async Task<BaseResponse<TicketDto>> UpdateStatusAsync(
            UpdateTicketStatusCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            if (currentUserRole == UserRole.Agent && ticket.AssignedAgentId != currentUserId)
                return BaseResponse<TicketDto>.Fail("You can only update tickets assigned to you.");

            // Validate transition
            var allowed = new Dictionary<TicketStatus, TicketStatus[]>
            {
                [TicketStatus.Open] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
                [TicketStatus.InProgress] = new[] { TicketStatus.OnHold, TicketStatus.Resolved },
                [TicketStatus.OnHold] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
                [TicketStatus.Resolved] = new[] { TicketStatus.Closed, TicketStatus.Reopened },
                [TicketStatus.Closed] = new[] { TicketStatus.Reopened },
                [TicketStatus.Reopened] = new[] { TicketStatus.InProgress, TicketStatus.Closed },
            };

            if (!allowed.TryGetValue(ticket.Status, out var validNext) || !validNext.Contains(command.NewStatus))
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
                    slaRecord.PausedAt = DateTime.UtcNow;
                else if (command.NewStatus == TicketStatus.InProgress && slaRecord.PausedAt is not null)
                {
                    slaRecord.TotalPausedMinutes += (int)(DateTime.UtcNow - slaRecord.PausedAt.Value).TotalMinutes;
                    slaRecord.PausedAt = null;
                }
                _uow.Sla.Update(slaRecord);
            }

            if (command.NewStatus == TicketStatus.Reopened)
            {
                ticket.ReopenCount++;
                ticket.IsEscalated = false; // reset escalation on reopen
            }

            ticket.Status = command.NewStatus;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
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

            var result = new PagedResult<TicketDto>
            {
                Items = _mapper.Map<List<TicketDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
            return BaseResponse<PagedResult<TicketDto>>.Ok(result);
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
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
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
            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Ticket closed.");
        }

        public async Task<BaseResponse<TicketDto>> UpdatePriorityAsync(Guid ticketId, TicketPriority priority)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<TicketDto>.Fail("Ticket not found.");

            ticket.Priority = priority;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(updated), "Priority updated.");
        }
    }
}
