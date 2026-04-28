using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationEventType EventType { get; set; }
    public bool IsEnabled { get; set; } = true;


    public User User { get; set; } = null!;
}