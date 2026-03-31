using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;
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
