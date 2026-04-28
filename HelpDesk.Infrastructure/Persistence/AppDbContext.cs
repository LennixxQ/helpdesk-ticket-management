using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace HelpDesk.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly ICurrentUserProvider? _currentUserProvider;


        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null, ICurrentUserProvider? currentUserProvider = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserProvider = currentUserProvider;
        }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<AuditLogDetail> AuditLogDetails => Set<AuditLogDetail>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();
        public DbSet<SlaRecord> SlaRecords => Set<SlaRecord>();
        public DbSet<EscalationRecord> EscalationRecords => Set<EscalationRecord>();
        public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
        public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
        public DbSet<KbArticle> KbArticles => Set<KbArticle>();
        public DbSet<KbArticleVersion> KbArticleVersions => Set<KbArticleVersion>();
        public DbSet<CsatResponse> CsatResponses => Set<CsatResponse>();
        public DbSet<RecurringTemplate> RecurringTemplates => Set<RecurringTemplate>();
        public DbSet<RecurringTemplateRun> RecurringTemplateRuns => Set<RecurringTemplateRun>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = GetCurrentUser();
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = now;
                    entry.Entity.LastModifiedBy = currentUser;
                }
            }
            foreach (var entry in ChangeTracker.Entries<User>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = now;
                    entry.Entity.LastModifiedBy = currentUser;
                }
            }
            var auditEntries = GenerateAuditEntries();
            var result = await base.SaveChangesAsync(cancellationToken);
            await FinalizeAuditEntriesAsync(auditEntries);
            if (auditEntries.Any())
                await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private string GetCurrentUser() =>
            _currentUserProvider?.GetCurrentUserId().ToString()?? _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM-ADMIN";

        private List<AuditEntry> GenerateAuditEntries()
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();
            var currentUser = GetCurrentUser();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is AuditLogDetail)
                    continue;
                if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
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
                        auditEntry.EntityId = Guid.TryParse(prop.CurrentValue?.ToString(), out var gid) ? gid : Guid.Empty;
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
                            auditEntry.Details.Add(detail);
                            break;
                        case EntityState.Deleted:
                            detail.OldValue = prop.OriginalValue?.ToString();
                            detail.NewValue = null;
                            auditEntry.Details.Add(detail);
                            break;
                        case EntityState.Modified when prop.IsModified:
                            detail.OldValue = prop.OriginalValue?.ToString();
                            detail.NewValue = prop.CurrentValue?.ToString();
                            auditEntry.Details.Add(detail);
                            continue;
                    }
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
                    IpAddress = entry.IpAddress,
                    Details = entry.Details
                };

                foreach (var detail in entry.Details)
                    detail.AuditLogId = log.Id;

                AuditLogs.Add(log);
            }

            return Task.CompletedTask;
        }
    }
}
