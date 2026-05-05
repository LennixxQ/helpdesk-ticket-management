using HelpDesk.Application.DTOs.AuditLogQuery;
using HelpDesk.Application.DTOs.Export;
using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByEntityIdAsync(Guid entityId);
        Task<List<AuditLogExportDto>> GetForExportAsync(DateTime from, DateTime to);
        Task<(List<AuditLogQueryDto> Items, int Total)> GetPagedAsync(DateTime? from, DateTime? to,string? actor, string? action, string? entityType,int page, int pageSize);
    }
}
