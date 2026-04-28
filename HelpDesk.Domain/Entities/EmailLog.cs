using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities;

public class EmailLog : BaseEntity
{
    public Guid RecipientUserId { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public int AttemptCount { get; set; } = 1;
    public string? FailureReason { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;


    public User Recipient { get; set; } = null!;
}