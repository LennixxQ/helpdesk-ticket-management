namespace HelpDesk.Application.Commands.CsatCommand
{
    public class SubmitCsatCommand
    {
        public Guid TicketId { get; set; }
        public int Score { get; set; }
        public string? Comments { get; set; }
    }
}
