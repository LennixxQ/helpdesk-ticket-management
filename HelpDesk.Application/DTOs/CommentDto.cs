namespace HelpDesk.Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string PostedByUserName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid TicketId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
