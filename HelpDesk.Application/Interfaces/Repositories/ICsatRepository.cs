using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ICsatRepository : IGenericRepository<CsatResponse>
{
    Task<CsatResponse?> GetByTicketIdAsync(Guid ticketId);
    Task<IEnumerable<CsatResponse>> GetByAgentIdAsync(Guid agentId);
    Task<double?> GetAverageScoreForAgentAsync(Guid agentId, DateTime from, DateTime to);
    Task<bool> ExistsForTicketAsync(Guid ticketId);
}