using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public class NotificationPreferenceRepository : GenericRepository<NotificationPreference>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<bool> IsEnabledAsync(Guid userId, NotificationEventType eventType)
    {
        var pref = await _context.NotificationPreferences.FirstOrDefaultAsync(n => n.UserId == userId && n.EventType == eventType);
        return pref?.IsEnabled ?? true;
    }

    public async Task UpsertAsync(Guid userId, NotificationEventType eventType, bool isEnabled)
    {
        var existing = await _context.NotificationPreferences.FirstOrDefaultAsync(n => n.UserId == userId && n.EventType == eventType);
        if (existing is null)
            await _context.NotificationPreferences.AddAsync(new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventType = eventType,
                IsEnabled = isEnabled,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            });
        else
        {
            existing.IsEnabled = isEnabled;
            existing.LastModifiedAt = DateTime.UtcNow;
            _context.NotificationPreferences.Update(existing);
        }
    }
}