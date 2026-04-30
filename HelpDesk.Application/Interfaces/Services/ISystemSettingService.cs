using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.SystemSetting;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<BaseResponse<List<SystemSettingDto>>> GetAllAsync();
        Task<BaseResponse<SystemSettingDto>> GetByKeyAsync(string key);
        Task<BaseResponse<object>> UpdateAsync(string key, string value, Guid adminId);
    }
}
