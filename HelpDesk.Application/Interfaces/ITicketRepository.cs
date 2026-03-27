using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.GenericInterface;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, TicketStatus? status = null, TicketPriority? priority = null,
            Guid? assignedAgentId = null, Guid? raisedByUserId = null, string? searchTerm = null, CancellationToken ct = default);

        Task<IEnumerable<Ticket>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default);

        Task<Dictionary<TicketStatus, int>> GetStatusCountsAsync(CancellationToken ct = default);

        Task<int> GetOpenCountAsync(CancellationToken ct = default);
    }
}
