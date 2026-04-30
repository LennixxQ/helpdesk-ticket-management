using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Identity;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.Infrastructure.Persistence.Repositories;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("HelpDesk.Infrastructure")
                ));

            services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IKbArticleService, KbArticleService>();
            services.AddScoped<IEscalationService, EscalationService>();
            services.AddScoped<ICsatService, CsatService>();
            services.AddScoped<IRecurringTemplateService, RecurringTemplateService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();

            return services;
        }
    }
}
