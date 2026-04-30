using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ISystemSettingRepository : IGenericRepository<SystemSetting>
{
    Task<SystemSetting?> GetByKeyAsync(string key);
}