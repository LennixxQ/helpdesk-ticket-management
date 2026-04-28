using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.Persistence.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var context = serviceProvider.GetRequiredService<AppDbContext>();
                string[] roles = { "Admin", "Agent", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>
                        {
                            Id = Guid.NewGuid(),
                            Name = role,
                            NormalizedName = role.ToUpper()
                        });
                        logger.LogInformation("Seeded role: {Role}", role);
                    }
                }
                const string adminEmail = "vivek.chauhan@helpdesk.com";
                var existing = await userManager.FindByEmailAsync(adminEmail);

                if (existing is null)
                {
                    var admin = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = "Vivek Chauhan",
                        Email = adminEmail,
                        UserName = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        NormalizedUserName = adminEmail.ToUpper(),
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "Vivek",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
                        logger.LogInformation("Seeded admin user: {Email}", adminEmail);
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("Failed to seed admin: {Errors}", errors);
                    }
                }
                var defaultCategories = new[] { "Hardware", "Software", "Network", "HR", "Other" };
                foreach (var name in defaultCategories)
                {
                    if (!context.Categories.Any(c => c.Name == name))
                    {
                        context.Categories.Add(new Domain.Entities.Category
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "seed"
                        });
                    }
                }

                if (!await context.Departments.AnyAsync(d => d.Name == "General"))
                {
                    context.Departments.Add(new Department
                    {
                        Id = Guid.NewGuid(),
                        Name = "General",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "Vivek-ADMIN",
                        LastModifiedAt = DateTime.Now,
                        LastModifiedBy = "Vivek-ADMIN"
                    });
                }

                if (!await context.SlaPolicies.AnyAsync())
                {
                    var policies = new List<SlaPolicy>
                {
                    new() { Id = Guid.NewGuid(), Priority = TicketPriority.Critical, FirstResponseMinutes = 60,   ResolutionMinutes = 240,  IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "Vivek-ADMIN", LastModifiedAt = DateTime.UtcNow, LastModifiedBy = "Vivek-ADMIN" },
                    new() { Id = Guid.NewGuid(), Priority = TicketPriority.High,     FirstResponseMinutes = 240,  ResolutionMinutes = 480,  IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "Vivek-ADMIN", LastModifiedAt = DateTime.UtcNow, LastModifiedBy = "Vivek-ADMIN" },
                    new() { Id = Guid.NewGuid(), Priority = TicketPriority.Medium,   FirstResponseMinutes = 480,  ResolutionMinutes = 4320, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "Vivek-ADMIN", LastModifiedAt = DateTime.UtcNow, LastModifiedBy = "Vivek-ADMIN" },
                    new() { Id = Guid.NewGuid(), Priority = TicketPriority.Low,      FirstResponseMinutes = 1440, ResolutionMinutes = 7200, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "Vivek-ADMIN", LastModifiedAt = DateTime.UtcNow, LastModifiedBy = "Vivek-ADMIN" },
                };
                    context.SlaPolicies.AddRange(policies);
                }

                if (!await context.SystemSettings.AnyAsync())
            {
                var settings = new List<SystemSetting>
                {
                    new() { Id = Guid.NewGuid(), Key = "SystemName",              Value = "HelpDesk",            Description = "Display name shown in header and emails",          UpdatedAt = DateTime.UtcNow },
                    new() { Id = Guid.NewGuid(), Key = "SupportEmail",            Value = "chauhanvivek1800@gmail.com", Description = "From address used in all outgoing notifications", UpdatedAt = DateTime.UtcNow },
                    new() { Id = Guid.NewGuid(), Key = "DefaultTimeZone",         Value = "UTC",                  Description = "Timezone for dashboard and report display",        UpdatedAt = DateTime.UtcNow },
                    new() { Id = Guid.NewGuid(), Key = "SessionTimeoutMinutes",   Value = "60",                   Description = "Inactivity timeout in minutes (15–480)",           UpdatedAt = DateTime.UtcNow },
                    new() { Id = Guid.NewGuid(), Key = "SurveyEmailDelayMinutes", Value = "30",                   Description = "Delay after ticket close before CSAT email sent",  UpdatedAt = DateTime.UtcNow },
                    new() { Id = Guid.NewGuid(), Key = "ArchivalPolicyMonths",    Value = "0",                    Description = "Months after which closed tickets are archived (0 = disabled)", UpdatedAt = DateTime.UtcNow },
                };
                context.SystemSettings.AddRange(settings);
            }

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeding completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database seeding.");
                throw;
            }
        }
    }
}
