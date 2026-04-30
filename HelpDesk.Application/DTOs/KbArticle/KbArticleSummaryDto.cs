using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs.KbArticle
{
    public class KbArticleSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public KbArticleStatus Status { get; set; }
    }
}
