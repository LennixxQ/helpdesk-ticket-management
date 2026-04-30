namespace HelpDesk.Application.Commands.KbCommand
{
    public class CreateKbArticleCommand
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public Guid CategoryId { get; set; }
    }
}
