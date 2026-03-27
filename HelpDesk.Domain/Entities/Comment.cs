namespace HelpDesk.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public User User { get; set; } = null!;
}