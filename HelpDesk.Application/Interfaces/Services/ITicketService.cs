using HelpDesk.Application.Commands.CommentCommand;
using HelpDesk.Application.Commands.TicketCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Comment;
using HelpDesk.Application.DTOs.Ticket;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<BaseResponse<CreateTicketResponseDto>> CreateAsync(CreateTicketCommand command, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<TicketDto>> AssignAsync(AssignTicketCommand command);
        Task<BaseResponse<TicketDto>> UpdateStatusAsync(UpdateTicketStatusCommand command, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<CommentDto>> AddCommentAsync(AddCommentCommand command, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<TicketDto>> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<PagedResult<TicketDto>>> GetAllAsync(PaginationDto dto, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<TicketDto>> ReopenAsync(Guid ticketId);
        Task<BaseResponse<TicketDto>> CloseAsync(Guid ticketId);
        Task<BaseResponse<TicketDto>> UpdatePriorityAsync(Guid ticketId, TicketPriority priority);
    }
}
