using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface INotificationPreferenceRepository : IGenericRepository<NotificationPreference>
{
    Task<bool> IsEnabledAsync(Guid userId, NotificationEventType eventType);
    Task UpsertAsync(Guid userId, NotificationEventType eventType, bool isEnabled);
}