using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public class CsatRepository : GenericRepository<CsatResponse>, ICsatRepository
{
    public CsatRepository(AppDbContext context) : base(context) { }

    public async Task<CsatResponse?> GetByTicketIdAsync(Guid ticketId) => 
        await _context.CsatResponses.Include(c => c.Respondent).FirstOrDefaultAsync(c => c.TicketId == ticketId);

    public async Task<IEnumerable<CsatResponse>> GetByAgentIdAsync(Guid agentId) => 
        await _context.CsatResponses.Where(c => c.ClosingAgentId == agentId).Include(c => c.Ticket).OrderByDescending(c => c.SubmittedAt).ToListAsync();

    public async Task<double?> GetAverageScoreForAgentAsync(Guid agentId, DateTime from, DateTime to)
    {
        var responses = await _context.CsatResponses.Where(c => c.ClosingAgentId == agentId && c.SubmittedAt >= from && c.SubmittedAt <= to).ToListAsync();
        return responses.Count < 5 ? null : responses.Average(c => c.Score);
    }

    public async Task<bool> ExistsForTicketAsync(Guid ticketId) =>
        await _context.CsatResponses.AnyAsync(c => c.TicketId == ticketId);
}