using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Repositories;

public interface IKbArticleRepository
{
    Task<KbArticle?> GetByIdAsync(Guid id);
    Task<IEnumerable<KbArticle>> GetAllAsync(KbArticleStatus? status = null);
    Task<IEnumerable<KbArticle>> SearchAsync(string keyword);
    Task<IEnumerable<KbArticle>> SuggestForTitleAsync(string ticketTitle, int take = 3);
    Task AddAsync(KbArticle article);
    void Update(KbArticle article);
    void Delete(KbArticle article);
}