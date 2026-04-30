using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.KbArticle
{
    public class KbArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public KbArticleStatus Status { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public int VersionNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
