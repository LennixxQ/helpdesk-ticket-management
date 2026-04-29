using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface IRecurringTemplateRepository
{
    Task<RecurringTemplate?> GetByIdAsync(Guid id);
    Task<IEnumerable<RecurringTemplate>> GetAllAsync();
    Task<IEnumerable<RecurringTemplate>> GetDueTemplatesAsync();
    Task AddAsync(RecurringTemplate template);
    void Update(RecurringTemplate template);
    void Delete(RecurringTemplate template);
}