using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public Guid CategoryId { get; set; }
    public Guid RaisedByUserId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public Category Category { get; set; } = null!;
    public User RaisedByUser { get; set; } = null!;
    public User? AssignedAgent { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];

    public void Assign(Guid agentId)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot assign a closed ticket.");

        AssignedAgentId = agentId;
        Status = TicketStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TicketStatus newStatus)
    {
        ValidateStatusTransition(Status, newStatus);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateStatusTransition(TicketStatus current, TicketStatus next)
    {
        var allowed = new Dictionary<TicketStatus, TicketStatus[]>
        {
            [TicketStatus.Open] = [TicketStatus.InProgress, TicketStatus.Closed],
            [TicketStatus.InProgress] = [TicketStatus.OnHold, TicketStatus.Resolved],
            [TicketStatus.OnHold] = [TicketStatus.InProgress, TicketStatus.Closed],
            [TicketStatus.Resolved] = [TicketStatus.Closed, TicketStatus.Reopened],
            [TicketStatus.Closed] = [TicketStatus.Reopened],
            [TicketStatus.Reopened] = [TicketStatus.InProgress, TicketStatus.Closed],
        };

        if (!allowed.TryGetValue(current, out var validNext) || !validNext.Contains(next))
            throw new DomainException($"Invalid status transition from '{current}' to '{next}'.");
    }
}