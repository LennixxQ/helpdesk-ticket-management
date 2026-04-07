using AutoMapper;
using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
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
        private readonly ICurrentUserProvider _currentUser;


        public TicketService(IUnitOfWork uow, IMapper mapper, ICurrentUserProvider currentUserProvider)
        {
            _uow = uow;
            _mapper = mapper;
            _currentUser = currentUserProvider;
        }

        public async Task<BaseResponse<CommentDto>> AddCommentAsync(AddCommentCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var validator = new AddCommentValidator();
            var result = await validator.ValidateAsync(command);
            if (!result.IsValid)
                return BaseResponse<CommentDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null)
                return BaseResponse<CommentDto>.Fail("Ticket not found");

            var isInvolved = currentUserRole == UserRole.Admin || ticket.RaisedByUserId == currentUserId || ticket.AssignedAgentId == currentUserId;
            if (!isInvolved)
                return BaseResponse<CommentDto>.Fail("You are not authorized to comment on this ticket.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TicketId = command.TicketId,
                UserId = currentUserId,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };

            ticket.LastModifiedAt = DateTime.UtcNow;
            ticket.LastModifiedBy = currentUserId.ToString();


            await _uow.Comments.AddAsync(comment);
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();
            comment.User = (await _uow.Users.GetByIdAsync(currentUserId))!;

            return BaseResponse<CommentDto>.Ok(_mapper.Map<CommentDto>(comment), "Comment Added");
        }

        public async Task<BaseResponse<TicketDto>> AssignAsync(AssignTicketCommand command)
        {
            var validator = new AssignTicketValidator();
            var result = await validator.ValidateAsync(command);

            if (!result.IsValid)
                return BaseResponse<TicketDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");

            var agent = await _uow.Users.GetByIdAsync(command.AgentId);
            if (agent is null || !agent.IsActive || agent.Role != UserRole.Agent)
                return BaseResponse<TicketDto>.Fail("Agent not found or inactive.");

            ticket.AssignedAgentId = command.AgentId;
            ticket.Status = TicketStatus.InProgress;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket), "Ticket assigned successfully.");
        }

        public async Task<BaseResponse<TicketDto>> CloseAsync(Guid ticketId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");

            ticket.Status = TicketStatus.Closed;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket), "Ticket closed.");
        }

        public async Task<BaseResponse<CreateTicketResponseDto>> CreateAsync(CreateTicketCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var validator = new CreateTicketValidator();
            var result = await validator.ValidateAsync(command);
            if (!result.IsValid)
                return BaseResponse<CreateTicketResponseDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var raisedById = currentUserRole == UserRole.Admin && command.RaisedByUserId.HasValue ? command.RaisedByUserId.Value : currentUserId;

            var ticket = _mapper.Map<Ticket>(command);
            ticket.Id = Guid.NewGuid();
            ticket.RaisedByUserId = raisedById;
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.CreatedBy = currentUserId.ToString();

            await _uow.Tickets.AddAsync(ticket);
            await _uow.SaveChangesAsync();

            
            var created = await _uow.Tickets.GetByIdWithDetailsAsync(ticket.Id);
            return BaseResponse<CreateTicketResponseDto>.Ok(new CreateTicketResponseDto { Id = ticket.Id, Status = ticket.Status.ToString()}, "Ticket created successfully.");

        }

        public async Task<BaseResponse<PagedResult<TicketDto>>> GetAllAsync(int page, int pageSize, TicketStatus? status, TicketPriority? priority, Guid? categoryId, Guid? agentId, Guid currentUserId, UserRole currentUserRole)
        {
            Guid? filterByUser = currentUserRole == UserRole.User ? currentUserId : null;
            Guid? filterByAgent = currentUserRole == UserRole.Agent ? currentUserId : agentId;


            var paged = await _uow.Tickets.GetAllPagedAsync(
                page, pageSize, status, priority, categoryId, filterByAgent, filterByUser);

            var dto = new PagedResult<TicketDto>
            {
                Items = _mapper.Map<List<TicketDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };

            return BaseResponse<PagedResult<TicketDto>>.Ok(dto);
        }

        public async Task<BaseResponse<TicketDto>> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(id);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");

            if (currentUserRole == UserRole.User && ticket.RaisedByUserId != currentUserId)
                return BaseResponse<TicketDto>.Fail("Access denied.");

            if (currentUserRole == UserRole.Agent && ticket.AssignedAgentId != currentUserId)
                return BaseResponse<TicketDto>.Fail("Access denied.");

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket));
        }

        public async Task<BaseResponse<TicketDto>> ReopenAsync(Guid ticketId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");

            if (ticket.Status != TicketStatus.Closed)
                return BaseResponse<TicketDto>.Fail("Only closed tickets can be reopened.");

            ticket.Status = TicketStatus.Reopened;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket), "Ticket reopened.");
        }

        public async Task<BaseResponse<TicketDto>> UpdatePriorityAsync(Guid ticketId, TicketPriority priority)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");

            ticket.Priority = priority;
            ticket.LastModifiedAt = DateTime.UtcNow;

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket), "Priority updated.");
        }

        public async Task<BaseResponse<TicketDto>> UpdateStatusAsync(UpdateTicketStatusCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var validator = new UpdateTicketStatusValidator();
            var result = await validator.ValidateAsync(command);
            if (!result.IsValid)
                return BaseResponse<TicketDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null)
                return BaseResponse<TicketDto>.Fail("Ticket not found.");
            if (currentUserRole == UserRole.Agent)
            {
                if (ticket.AssignedAgentId != currentUserId)
                    return BaseResponse<TicketDto>.Fail("You can only update tickets assigned to you.");

                var agentAllowed = new[] { TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.Resolved };
                if (!agentAllowed.Contains(command.NewStatus))
                    return BaseResponse<TicketDto>.Fail("Agents can only set status to InProgress, OnHold, or Resolved.");
            }

            if (command.NewStatus == TicketStatus.Closed && currentUserRole != UserRole.Admin)
                return BaseResponse<TicketDto>.Fail("Only Admin can close a ticket.");

            ticket.Status = command.NewStatus;
            ticket.LastModifiedAt = DateTime.UtcNow;
            ticket.LastModifiedBy = currentUserId.ToString();

            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<TicketDto>.Ok(_mapper.Map<TicketDto>(ticket), "Status updated.");
        }
    }
}
