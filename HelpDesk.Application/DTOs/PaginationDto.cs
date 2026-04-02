using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs
{
    public class PaginationDto
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public TicketStatus? status { get; set; } = null;
        public TicketPriority? priority { get; set; } = null;
        public Guid? categoryId { get; set; } = null;
        public Guid? agentId { get; set; } = null;
    }
}
