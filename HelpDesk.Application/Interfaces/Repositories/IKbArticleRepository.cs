using HelpDesk.Application.Interfaces.Repositories.GenericInterface;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface IKbArticleRepository : IGenericRepository<KbArticle>
{
    Task<KbArticle?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<KbArticle>> GetByStatusAsync(KbArticleStatus status);
    Task<IEnumerable<KbArticle>> SearchAsync(string keyword);
    Task<IEnumerable<KbArticle>> SuggestForTitleAsync(string title, int take = 3);
}