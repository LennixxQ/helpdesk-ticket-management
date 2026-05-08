using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public class KbArticleVersionRepository : IKbArticleVersionRepository
{
    private readonly AppDbContext _context;

    public KbArticleVersionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(KbArticleVersion version)
    {
        await _context.KbArticleVersions.AddAsync(version);
    }

    public async Task<IEnumerable<KbArticleVersion>> GetByArticleIdAsync(Guid articleId)
    {
        return await _context.KbArticleVersions
            .Where(v => v.KbArticleId == articleId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }
}
