using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface INotificationPreferenceRepository
{
    Task<IEnumerable<NotificationPreference>> GetByUserIdAsync(Guid userId);
    Task<bool> IsEnabledAsync(Guid userId, NotificationEventType eventType);
    Task UpsertAsync(Guid userId, NotificationEventType eventType, bool isEnabled);
}