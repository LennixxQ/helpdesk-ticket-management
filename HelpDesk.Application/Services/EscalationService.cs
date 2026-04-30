using AutoMapper;
using HelpDesk.Application.Commands.EscalationCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Escalation;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class EscalationService : IEscalationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly EscalateTicketValidator _validator;

        public EscalationService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = new EscalateTicketValidator();
        }

        public async Task<BaseResponse<EscalationDto>> EscalateAsync(
            EscalateTicketCommand command, Guid currentUserId)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<EscalationDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(command.TicketId);
            if (ticket is null) return BaseResponse<EscalationDto>.Fail("Ticket not found.");

            if (ticket.IsEscalated)
                return BaseResponse<EscalationDto>.Fail("Ticket is already escalated.");

            var user = await _uow.Users.GetByIdAsync(currentUserId);
            if (user is null) return BaseResponse<EscalationDto>.Fail("User not found.");

            if (user.Role == UserRole.Agent && ticket.AssignedAgentId != currentUserId)
                return BaseResponse<EscalationDto>.Fail("Agents can only escalate their assigned tickets.");

            // Auto-raise priority
            ticket.Priority = ticket.Priority switch
            {
                TicketPriority.Low => TicketPriority.Medium,
                TicketPriority.Medium => TicketPriority.High,
                TicketPriority.High => TicketPriority.Critical,
                _ => ticket.Priority
            };

            ticket.IsEscalated = true;
            ticket.LastModifiedAt = DateTime.UtcNow;

            var escalation = new EscalationRecord
            {
                Id = Guid.NewGuid(),
                TicketId = command.TicketId,
                Reason = command.Reason,
                Trigger = EscalationTrigger.Manual,
                EscalatedBy = user.FullName,
                EscalatedByUserId = currentUserId,
                EscalatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };

            ticket.EscalationRecord = escalation;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            escalation.EscalatedByUser = user;
            return BaseResponse<EscalationDto>.Ok(_mapper.Map<EscalationDto>(escalation), "Ticket escalated.");
        }

        public async Task<BaseResponse<object>> AcknowledgeAsync(Guid ticketId, Guid currentUserId)
        {
            var ticket = await _uow.Tickets.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null) return BaseResponse<object>.Fail("Ticket not found.");
            if (!ticket.IsEscalated || ticket.EscalationRecord is null)
                return BaseResponse<object>.Fail("Ticket is not escalated.");
            if (ticket.EscalationRecord.AcknowledgedAt.HasValue)
                return BaseResponse<object>.Fail("Already acknowledged.");

            ticket.EscalationRecord.AcknowledgedByUserId = currentUserId;
            ticket.EscalationRecord.AcknowledgedAt = DateTime.UtcNow;
            ticket.LastModifiedAt = DateTime.UtcNow;
            _uow.Tickets.Update(ticket);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), "Escalation acknowledged.");
        }
    }
}
