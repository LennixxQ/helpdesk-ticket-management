using HelpDesk.Application.Commands.RecurringTemplateCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.RecurringTemplate;

namespace HelpDesk.Application.Interfaces.Services;

public interface IRecurringTemplateService
{
    Task<BaseResponse<List<RecurringTemplateDto>>> GetAllAsync();
    Task<BaseResponse<RecurringTemplateDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<RecurringTemplateDto>> CreateAsync(CreateRecurringTemplateCommand command, Guid adminId);
    Task<BaseResponse<object>> ToggleActiveAsync(Guid id);
    Task<BaseResponse<object>> TriggerManualAsync(Guid id, Guid adminId);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}