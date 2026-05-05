using HelpDesk.Application.DTOs.Report;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ITicketReportRepository
    {
        Task<List<Ticket>> GetForReportAsync(ReportFilterDto filter);
        Task<List<Ticket>> GetArchivedPagedAsync(int page, int pageSize);
        Task<int> GetArchivedCountAsync();
        Task<int> GetCommentCountByAgentAsync(Guid agentId, DateTime from, DateTime to);
    }
}
