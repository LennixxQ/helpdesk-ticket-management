using AutoMapper;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.SystemSetting;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;

namespace HelpDesk.Application.Services
{
    public class SystemSettingService :ISystemSettingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SystemSettingService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<SystemSettingDto>>> GetAllAsync()
        {
            var settings = await _uow.SystemSettings.GetAllAsync();
            return BaseResponse<List<SystemSettingDto>>.Ok(_mapper.Map<List<SystemSettingDto>>(settings));
        }

        public async Task<BaseResponse<SystemSettingDto>> GetByKeyAsync(string key)
        {
            var setting = await _uow.SystemSettings.GetByKeyAsync(key);
            if (setting is null) return BaseResponse<SystemSettingDto>.Fail($"Setting '{key}' not found.");
            return BaseResponse<SystemSettingDto>.Ok(_mapper.Map<SystemSettingDto>(setting));
        }

        public async Task<BaseResponse<object>> UpdateAsync(string key, string value, Guid adminId)
        {
            var setting = await _uow.SystemSettings.GetByKeyAsync(key);
            if (setting is null) return BaseResponse<object>.Fail($"Setting '{key}' not found.");

            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedById = adminId;
            _uow.SystemSettings.Update(setting);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(), $"Setting '{key}' updated.");
        }
    }
}
