using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
        public Guid? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
