namespace HelpDesk.Application.Commands.CommentCommand
{
    public class AddCommentCommand
    {
        public Guid TicketId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
