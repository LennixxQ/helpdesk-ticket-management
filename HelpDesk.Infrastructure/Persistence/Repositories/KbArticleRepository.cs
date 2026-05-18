using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public class KbArticleRepository : GenericRepository<KbArticle>, IKbArticleRepository
{
    public KbArticleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<KbArticle>> GetAllWithIncludesAsync() =>
        await _context.KbArticles
            .Include(k => k.Author)
            .Include(k => k.Category)
            .Include(k => k.LastUpdatedBy)
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();

    public async Task<KbArticle?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.KbArticles.Include(k => k.Author).Include(k => k.LastUpdatedBy)
        .Include(k => k.Category).FirstOrDefaultAsync(k => k.Id == id);

    public async Task<IEnumerable<KbArticle>> GetByStatusAsync(KbArticleStatus status) =>
        await _context.KbArticles
            .Include(k => k.Author)
            .Include(k => k.Category)
            .Include(k => k.LastUpdatedBy)
            .Where(k => k.Status == status)
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<KbArticle>> SearchAsync(string keyword)
    {
        var term = keyword.Trim().ToLower();
        var title = await _context.KbArticles.Include(k => k.Category).Include(k => k.Author).Include(k => k.LastUpdatedBy).Where(k => k.Status == KbArticleStatus.Published && k.Title.ToLower().Contains(term)).ToListAsync();
        var body = await _context.KbArticles.Include(k => k.Category).Include(k => k.Author).Include(k => k.LastUpdatedBy).Where(k => k.Status == KbArticleStatus.Published && !k.Title.ToLower().Contains(term) && (k.Content.ToLower().Contains(term) || (k.Tags != null && k.Tags.ToLower().Contains(term)))).ToListAsync();
        return title.Concat(body);
    }

    public async Task<IEnumerable<KbArticle>> SuggestForTitleAsync(string title, int take = 3)
    {
        var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).Select(w => w.Trim().ToLower()).ToList();
        if (!words.Any()) return new List<KbArticle>();
        var articles = await _context.KbArticles.Where(k => k.Status == KbArticleStatus.Published).Include(k => k.Category).Include(k => k.Author).Include(k => k.LastUpdatedBy).ToListAsync();
        return articles
            .Select(a => new { Article = a, Score = words.Count(w => a.Title.ToLower().Contains(w) || (a.Tags != null && a.Tags.ToLower().Contains(w))) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score).Take(take)
            .Select(x => x.Article);
    }
}