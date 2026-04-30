using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class SlaRepository : GenericRepository<SlaRecord>, ISlaRepository
    {
        private readonly AppDbContext _context;

        public SlaRepository(AppDbContext context) : base (context)
        {
        }

        public async Task AddAsync(SlaRecord record) =>
            await _context.SlaRecords.AddAsync(record);

        public async Task<IEnumerable<SlaRecord>> GetBreachingRecordsAsync() =>
            await _context.SlaRecords.Include(s => s.Ticket)
            .Where(s => !s.IsBreached && s.PausedAt == null && s.SlaDeadline <= DateTime.UtcNow && s.Ticket.Status != TicketStatus.Closed && s.Ticket.Status != TicketStatus.Resolved)
            .ToListAsync();

        public async Task<SlaRecord?> GetByTicketIdAsync(Guid ticketId) =>
            await _context.SlaRecords.FirstOrDefaultAsync(s => s.TicketId == ticketId);

        public async Task<SlaPolicy?> GetPolicyByPriorityAsync(TicketPriority priority) =>
            await _context.SlaPolicies.FirstOrDefaultAsync(p => p.Priority == priority && p.IsActive);

        public async Task<IEnumerable<SlaRecord>> GetWarningRecordsAsync()
        {
            var now = DateTime.UtcNow;
            var records = await _context.SlaRecords
                .Include(s => s.Ticket)
                .Where(s => !s.IsBreached && s.Status != SlaStatus.Warning && s.PausedAt == null && s.Ticket.Status != TicketStatus.Closed && s.Ticket.Status != TicketStatus.Resolved)
                .ToListAsync();

            return records.Where(s => {
                var total = (s.SlaDeadline - s.CreatedAt).TotalMinutes;
                var elapsed = (now - s.CreatedAt).TotalMinutes - s.TotalPausedMinutes;
                return total > 0 && (elapsed / total) >= 0.75;
            });
        }

        public async void Update(SlaRecord record) => 
            _context.SlaRecords.Update(record);
    }
}
