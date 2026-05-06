namespace HelpDesk.Domain.Entities
{
    public class WelcomeEmailModel
    {
        public string SystemName { get; set; } = "HelpDesk";
        public string RecipientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TempPassword { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
    }
}
