namespace HelpDesk.Domain.Entities
{
    public class TicketEmailModel
    {
        public string SystemName { get; set; } = "HelpDesk";
        public string RecipientName { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public string TicketId { get; set; } = string.Empty;
        public string TicketTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusClass { get; set; } = "open";
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? AssignedAgent { get; set; }
        public DateTime? SlaDeadline { get; set; }
        public string TicketUrl { get; set; } = string.Empty;
        public string PreferencesUrl { get; set; } = string.Empty;
    }
}
