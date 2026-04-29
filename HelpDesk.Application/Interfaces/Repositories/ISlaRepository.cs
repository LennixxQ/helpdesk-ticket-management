using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface ISlaRepository
{
    Task<SlaRecord?> GetByTicketIdAsync(Guid ticketId);
    Task AddAsync(SlaRecord record);
    void Update(SlaRecord record);
    Task<SlaPolicy?> GetPolicyByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<SlaRecord>> GetBreachingRecordsAsync();
    Task<IEnumerable<SlaRecord>> GetWarningRecordsAsync();
}