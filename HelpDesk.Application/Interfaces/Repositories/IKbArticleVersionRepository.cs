using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface IKbArticleVersionRepository
{
    Task AddAsync(KbArticleVersion version);
    Task<IEnumerable<KbArticleVersion>> GetByArticleIdAsync(Guid articleId);
}