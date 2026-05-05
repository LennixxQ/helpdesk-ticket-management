namespace HelpDesk.Application.DTOs.Sla
{
    public class SlaOverrideRequest
    {
        public Guid TicketId { get; set; }
        public DateTime NewDeadline { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
