using HelpDesk.Application.Interfaces.Repositories;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ITicketRepository Tickets { get; }
        public IUserRepository Users { get; }
        public ICategoryRepository Categories { get; }
        public IAuditLogRepository AuditLogs { get; }
        public ICommentRepository Comments { get; }
        public IDepartmentRepository Departments { get; }
        //ISlaRepository Sla { get; }
        //IKbArticleRepository KbArticles { get; }
        //IKbArticleVersionRepository KbArticleVersions { get; }
        //ICsatRepository Csat { get; }
        //IRecurringTemplateRepository RecurringTemplates { get; }
        //ISystemSettingRepository SystemSettings { get; }
        //INotificationPreferenceRepository NotificationPreferences { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Tickets = new TicketRepository(context);
            Users = new UserRepository(context);
            Categories = new CategoryRepository(context);
            AuditLogs = new AuditLogRepository(context);
            Comments = new CommentRepository(context);
            Departments = new DepartmentRepository(context);
            //Sla = new SlaRepository(context);
            //KbArticles = new KbArticleRepository(context);
            //KbArticleVersions = new KbArticleVersionRepository(context);
            //Csat = new CsatRepository(context);
            //RecurringTemplates = new RecurringTemplateRepository(context);
            //SystemSettings = new SystemSettingRepository(context);
            //NotificationPreferences = new NotificationPreferenceRepository(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        } 


        public void Dispose() =>
            _context.Dispose();
    }
}
