using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public Guid RaisedByUserId { get; set; }
    public User RaisedByUser { get; set; } = null!;
    public Guid? AssignedAgentId { get; set; }
    public User? AssignedAgent { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}