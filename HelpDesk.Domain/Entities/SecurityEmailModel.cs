namespace HelpDesk.Domain.Entities
{
    public class SecurityEmailModel
    {
        public string SystemName { get; set; } = "HelpDesk";
        public string RecipientName { get; set; } = string.Empty;
        public DateTime ActionTimestamp { get; set; }
        public string ActionLocation { get; set; } = "Unknown"; // Future use: IP/Location
        public string TimeZoneName { get; set; } = "UTC";
    }
}
