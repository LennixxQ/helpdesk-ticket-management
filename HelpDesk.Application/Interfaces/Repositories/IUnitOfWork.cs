namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IAuditLogRepository AuditLogs { get; }
        ICommentRepository Comments { get; }
        Task<int> SaveChangesAsync();
    }
}
