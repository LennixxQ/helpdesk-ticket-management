namespace HelpDesk.Application.Commands.KbCommand
{
    public class UpdateKbArticleCommand
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public Guid CategoryId { get; set; }
    }
}
