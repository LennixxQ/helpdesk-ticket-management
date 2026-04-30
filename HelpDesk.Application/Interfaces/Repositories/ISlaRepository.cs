using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ISlaRepository : IGenericRepository<SlaRecord>
{
    Task<SlaRecord?> GetByTicketIdAsync(Guid ticketId);
    Task<SlaPolicy?> GetPolicyByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<SlaRecord>> GetBreachingRecordsAsync();
    Task<IEnumerable<SlaRecord>> GetWarningRecordsAsync();
}