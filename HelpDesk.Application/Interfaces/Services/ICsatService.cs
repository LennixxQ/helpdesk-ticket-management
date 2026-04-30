using HelpDesk.Application.Commands.CsatCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Csat;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ICsatService
    {
        Task<BaseResponse<object>> SubmitAsync(SubmitCsatCommand command, Guid currentUserId);
        Task<BaseResponse<AgentCsatStatsDto>> GetAgentStatsAsync(Guid agentId, DateTime from, DateTime to);
    }
}
