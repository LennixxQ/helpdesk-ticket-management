using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Commands.TicketCommand
{
    public class CreateTicketCommand
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public TicketPriority Priority { get; set; }
        public Guid? RaisedByUserId { get; set; }
    }
}
