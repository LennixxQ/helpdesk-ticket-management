using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities;

public class CsatResponse : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid RespondentId { get; set; }
    public Guid ClosingAgentId { get; set; }
    public int Score { get; set; }
    public string? Comments { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
    public User Respondent { get; set; } = null!;
    public User ClosingAgent { get; set; } = null!;
}