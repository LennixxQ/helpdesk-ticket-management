using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace HelpDesk.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<AuditLogDetail> AuditLogDetails => Set<AuditLogDetail>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = GenerateAuditEntries();
            var result = await base.SaveChangesAsync(cancellationToken);
            await FinalizeAuditEntriesAsync(auditEntries);
            if (auditEntries.Any())
                await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        private List<AuditEntry> GenerateAuditEntries()
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();

            var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system";

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is AuditLogDetail)
                    continue;
                if (entry.State is not (EntityState.Added
                    or EntityState.Modified
                    or EntityState.Deleted))
                    continue;

                var auditEntry = new AuditEntry
                {
                    EntityName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    PerformedBy = currentUser,
                    PerformedAt = DateTime.UtcNow,
                    IpAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.EntityId = Guid.TryParse(prop.CurrentValue?.ToString(), out var gid)
                            ? gid : Guid.Empty;
                        continue;
                    }

                    var detail = new AuditLogDetail
                    {
                        Id = Guid.NewGuid(),
                        FieldName = prop.Metadata.Name
                    };

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            detail.OldValue = null;
                            detail.NewValue = prop.CurrentValue?.ToString();
                            break;
                        case EntityState.Deleted:
                            detail.OldValue = prop.OriginalValue?.ToString();
                            detail.NewValue = null;
                            break;
                        case EntityState.Modified when prop.IsModified:
                            detail.OldValue = prop.OriginalValue?.ToString();
                            detail.NewValue = prop.CurrentValue?.ToString();
                            auditEntry.Details.Add(detail);
                            continue;
                    }

                    if (entry.State != EntityState.Modified)
                        auditEntry.Details.Add(detail);
                }

                entries.Add(auditEntry);
            }

            return entries;
        }

        private Task FinalizeAuditEntriesAsync(List<AuditEntry> entries)
        {
            foreach (var entry in entries)
            {
                var log = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = entry.EntityName,
                    EntityId = entry.EntityId,
                    Action = entry.Action,
                    PerformedBy = entry.PerformedBy,
                    PerformedAt = entry.PerformedAt,
                    IpAddress = entry.IpAddress
                };

                foreach (var detail in entry.Details)
                    detail.AuditLogId = log.Id;

                log.Details = entry.Details;
                AuditLogs.Add(log);
            }

            return Task.CompletedTask;
        }
    }
}
