using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActionAsync(string actionType, string entityType, Guid? entityId, string? notes = null)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var log = new AuditLog
            {
                Action = actionType,
                EntityName = entityType,
                EntityId = entityId ?? Guid.Empty,
                PerformedAt = DateTime.UtcNow,
                IpAddress = ip,
                AdditionalNotes = notes,
                
                // Actor details from Claims
                PerformedBy = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM",
                ActorName = user?.FindFirst("FullName")?.Value ?? user?.Identity?.Name ?? "SYSTEM",
                ActorEmail = user?.FindFirst(ClaimTypes.Email)?.Value ?? "",
                ActorRole = user?.FindFirst(ClaimTypes.Role)?.Value ?? ""
            };

            await _uow.AuditLogs.AddAsync(log);
            await _uow.SaveChangesAsync();
        }
    }
}
