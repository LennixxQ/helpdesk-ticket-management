using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace HelpDesk.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<AuditLogDetail> AuditLogDetails => Set<AuditLogDetail>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new TicketConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
            builder.ApplyConfiguration(new AuditLogConfiguration());
            builder.ApplyConfiguration(new AuditLogDetailConfiguration());

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

            SeedData(builder);
        }

        private static void SeedData(ModelBuilder builder)
        {
            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var agendRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var regularRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            builder.Entity<IdentityRole<Guid>>().HasData(new IdentityRole<Guid>
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = adminRoleId.ToString()
            }, new IdentityRole<Guid>
            {
                Id = agendRoleId,
                Name = "SupportAgent",
                NormalizedName = "SUPPORT-AGENT",
                ConcurrencyStamp = agendRoleId.ToString()
            }, new IdentityRole<Guid>
            {
                Id = regularRoleId,
                Name = "RegularUser",
                NormalizedName = "REGULAR-USER",
                ConcurrencyStamp = regularRoleId.ToString()
            });

            var adminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var hasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                Id = adminUserId,
                FirstName = "Vivek",
                LastName = "Chauhan",
                UserName = "vivek.chauhan@helpdesk.com",
                NormalizedUserName = "VIVEK.CHAUHAN@HELPDESK.COM",
                Email = "vivek.chauhan@helpdesk.com",
                NormalizedEmail = "VIVEK.CHAUHAN@HELPDESK.COM",
                EmailConfirmed = true,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            builder.Entity<User>().HasData(adminUser);

            builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                UserId = adminUser.Id,
                RoleId = adminRoleId
            });

            builder.Entity<Category>().HasData(new Category
            {
                Id = Guid.Parse("c1000000-0000-0000-0000-000000000001"),
                Name = "Hardware",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.Parse("c2000000-0000-0000-0000-000000000002"),
                Name = "Software",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.Parse("c3000000-0000-0000-0000-000000000003"),
                Name = "Network",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

            }, new Category
            {
                Id = Guid.Parse("c4000000-0000-0000-0000-000000000004"),
                Name = "Access Request",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.Parse("c5000000-0000-0000-0000-000000000005"),
                Name = "Other",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
    }
}
