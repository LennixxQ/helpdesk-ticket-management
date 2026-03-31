using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<BaseResponse<DashboardDto>> GetDashboardAsync();
    }
}
