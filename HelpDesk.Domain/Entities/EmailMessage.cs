namespace HelpDesk.Domain.Entities
{
    public class EmailMessage
    {
        public Guid RecipientUserId { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string PlainTextBody { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
    }
}
