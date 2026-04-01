using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;

namespace HelpDesk.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context) { }
    }
}
