using System;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(string actionType, string entityType, Guid? entityId, string? notes = null);
    }
}
