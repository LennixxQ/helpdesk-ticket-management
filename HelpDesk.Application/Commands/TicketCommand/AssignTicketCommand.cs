namespace HelpDesk.Application.Commands.TicketCommand
{
    public class AssignTicketCommand
    {
        public Guid TicketId { get; set; }
        public Guid AgentId { get; set; }
    }
}
