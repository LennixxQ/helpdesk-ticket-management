using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Dashboard;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;
        public DashboardService(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseResponse<DashboardDto>> GetDashboardAsync()
        {
            var statusCounts = new Dictionary<string, int>();
            var priorityCounts = new Dictionary<string, int>();

            foreach (TicketStatus s in Enum.GetValues<TicketStatus>())
                statusCounts[s.ToString()] = await _uow.Tickets.CountByStatusAsync(s);

            foreach (TicketPriority p in Enum.GetValues<TicketPriority>())
                priorityCounts[p.ToString()] = await _uow.Tickets.CountByPriorityAsync(p);

            var topAgentsRaw = await _uow.Tickets.GetTopAgentsThisMonthAsync(5);
            var topAgents = topAgentsRaw.Select(a => new TopAgentDto
            {
                AgentId = a.AgentId,
                AgentName = a.AgentName,
                ResolvedCount = a.Count
            }).ToList();

            return BaseResponse<DashboardDto>.Ok(new DashboardDto
            {
                TotalTickets = statusCounts.Values.Sum(),
                TicketsByStatus = statusCounts,
                TicketsByPriority = priorityCounts,
                TicketsThisMonth = await _uow.Tickets.CountThisMonthAsync(),
                TicketsLastMonth = await _uow.Tickets.CountLastMonthAsync(),
                TopAgentsThisMonth = topAgents
            });
        }
    }
}
