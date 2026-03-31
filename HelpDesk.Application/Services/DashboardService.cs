using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BaseResponse<DashboardDto>> GetDashboardAsync()
        {
            var allTickets = await _uow.Tickets.GetAllAsync();

            var byStatus = new Dictionary<string, int>();
            foreach (TicketStatus s in Enum.GetValues<TicketStatus>())
                byStatus[s.ToString()] = await _uow.Tickets.CountByStatusAsync(s);

            var byPriority = new Dictionary<string, int>();
            foreach (TicketPriority p in Enum.GetValues<TicketPriority>())
                byPriority[p.ToString()] = await _uow.Tickets.CountByPriorityAsync(p);

            var topAgents = await _uow.Tickets.GetTopAgentsThisMonthAsync(5);

            var dto = new DashboardDto
            {
                TotalTickets = allTickets.Count(),
                TicketsByStatus = byStatus,
                TicketsByPriority = byPriority,
                TicketsThisMonth = await _uow.Tickets.CountThisMonthAsync(),
                TicketsLastMonth = await _uow.Tickets.CountLastMonthAsync(),
                TopAgentsThisMonth = topAgents.Select(a => new TopAgentDto
                {
                    AgentId = a.AgentId,
                    AgentName = a.AgentName,
                    ResolvedCount = a.Count
                }).ToList()
            };

            return BaseResponse<DashboardDto>.Ok(dto);
        }
    }
}
