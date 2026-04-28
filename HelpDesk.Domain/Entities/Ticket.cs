using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public bool IsEscalated { get; set; } = false;
    public int ReopenCount { get; set; } = 0;
    public bool IsArchived { get; set; } = false;
    public DateTime? SlaDeadline { get; set; }
    public SlaStatus SlaStatus { get; set; } = SlaStatus.WithinSla;
    public bool SlaBreached { get; set; } = false;


    public Guid CategoryId { get; set; }
    public Guid RaisedByUserId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public Guid? DepartmentId { get; set; }


    public Category Category { get; set; } = null!;
    public User RaisedByUser { get; set; } = null!;
    public User? AssignedAgent { get; set; }
    public Department? Department { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public SlaRecord? SlaRecord { get; set; }
    public EscalationRecord? EscalationRecord { get; set; }
}