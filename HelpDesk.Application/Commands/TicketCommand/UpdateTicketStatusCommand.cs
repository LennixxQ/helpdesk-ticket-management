using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Commands.TicketCommand
{
    public class UpdateTicketStatusCommand
    {
        public Guid TicketId { get; set; }
        public TicketStatus NewStatus { get; set; }
        public Guid? KbArticleId { get; set; }
    }
}
