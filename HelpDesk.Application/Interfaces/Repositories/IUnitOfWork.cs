namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IAuditLogRepository AuditLogs { get; }
        ICommentRepository Comments { get; }
        IDepartmentRepository Departments { get; }
        ISlaRepository Sla { get; }
        IKbArticleRepository KbArticles { get; }
        IKbArticleVersionRepository KbArticleVersions { get; }
        ICsatRepository Csat { get; }
        IRecurringTemplateRepository RecurringTemplates { get; }
        ISystemSettingRepository SystemSettings { get; }
        INotificationPreferenceRepository NotificationPreferences { get; }
        Task<int> SaveChangesAsync();
    }
}
