using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.Priority
{
    public class UpdatePriorityRequest
    {
        public Guid TicketId { get; set; }
        public TicketPriority Priority { get; set; }
    }
}
