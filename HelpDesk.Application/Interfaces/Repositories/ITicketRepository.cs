using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket?> GetByIdWithDetailsAsync(Guid id);
        Task<PagedResult<Ticket>> GetAllPagedAsync(int page, int pageSize, TicketStatus? status = null, TicketPriority? priority = null,
            Guid? categoryId = null, Guid? agentId = null, Guid? raisedByUserId = null);
        Task<IEnumerable<Ticket>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Ticket>> GetByAgentIdAsync(Guid agentId);
        Task<int> CountByStatusAsync(TicketStatus status);
        Task<int> CountByPriorityAsync(TicketPriority priority);
        Task<int> CountThisMonthAsync();
        Task<int> CountLastMonthAsync();
        Task<List<(Guid AgentId, string AgentName, int Count)>> GetTopAgentsThisMonthAsync(int top = 5);
    }
}
