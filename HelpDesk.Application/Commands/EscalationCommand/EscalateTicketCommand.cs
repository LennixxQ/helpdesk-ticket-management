namespace HelpDesk.Application.Commands.EscalationCommand
{
    public class EscalateTicketCommand
    {
        public Guid TicketId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
