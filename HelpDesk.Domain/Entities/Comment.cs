using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }


    public Ticket Ticket { get; set; } = null!;
    public User User { get; set; } = null!;
}