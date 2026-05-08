using System;

namespace HelpDesk.Application.DTOs.KbArticle
{
    public class KbArticleVersionDto
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid SavedByUserId { get; set; }
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
    }
}
