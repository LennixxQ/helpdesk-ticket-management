using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ISystemSettingRepository
{
    Task<SystemSetting?> GetByKeyAsync(string key);
    Task<IEnumerable<SystemSetting>> GetAllAsync();
    void Update(SystemSetting setting);
}