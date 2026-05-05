using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public class RecurringTemplateRepository : GenericRepository<RecurringTemplate>, IRecurringTemplateRepository
{
    public RecurringTemplateRepository(AppDbContext context) : base(context) { }

    public override async Task<RecurringTemplate?> GetByIdAsync(Guid id) => 
        await _context.RecurringTemplates.Include(r => r.Category).Include(r => r.AssignToAgent)
        .Include(r => r.Runs.OrderByDescending(run => run.ActualRunAt).Take(5)).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<RecurringTemplate>> GetDueTemplatesAsync() => 
        await _context.RecurringTemplates.Include(r => r.Category).Include(r => r.AssignToAgent).Include(r => r.RaiseOnBehalfOf)
        .Where(r => r.IsActive && r.NextRunAt.HasValue && r.NextRunAt.Value <= DateTime.UtcNow && (r.EndDate == null || r.EndDate.Value >= DateTime.UtcNow) && (r.MaxOccurrences == null || r.RunCount < r.MaxOccurrences))
        .ToListAsync();
}