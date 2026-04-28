using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class KbArticle : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public KbArticleStatus Status { get; set; } = KbArticleStatus.Draft;
    public int ViewCount { get; set; } = 0;
    public int HelpfulCount { get; set; } = 0;
    public int NotHelpfulCount { get; set; } = 0;
    public int VersionNumber { get; set; } = 1;
    public Guid CategoryId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid LastUpdatedById { get; set; }

    public Category Category { get; set; } = null!;
    public User Author { get; set; } = null!;
    public User LastUpdatedBy { get; set; } = null!;
    public ICollection<KbArticleVersion> Versions { get; set; } = new List<KbArticleVersion>();
}