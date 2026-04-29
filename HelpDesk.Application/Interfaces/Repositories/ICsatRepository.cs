using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ICsatRepository
{
    Task<CsatResponse?> GetByTicketIdAsync(Guid ticketId);
    Task<IEnumerable<CsatResponse>> GetByAgentIdAsync(Guid agentId);
    Task<double?> GetAverageScoreForAgentAsync(Guid agentId, DateTime from, DateTime to);
    Task AddAsync(CsatResponse response);
    Task<bool> ExistsForTicketAsync(Guid ticketId);
}