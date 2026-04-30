using HelpDesk.Application.Commands.EscalationCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Escalation;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IEscalationService
    {
        Task<BaseResponse<EscalationDto>> EscalateAsync(EscalateTicketCommand command, Guid currentUserId);
        Task<BaseResponse<object>> AcknowledgeAsync(Guid ticketId, Guid currentUserId);
    }
}
