namespace HelpDesk.Domain.Entities;

public class KbArticleVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid KbArticleId { get; set; }
    public Guid SavedByUserId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    public KbArticle KbArticle { get; set; } = null!;
    public User SavedByUser { get; set; } = null!;
}