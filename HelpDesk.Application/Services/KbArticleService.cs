using AutoMapper;
using HelpDesk.Application.Commands.KbCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.KbArticle;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
namespace HelpDesk.Application.Services
{
    public class KbArticleService : IKbArticleService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly CreateKbArticleValidator _validator;

        public KbArticleService(IUnitOfWork uow, IMapper mapper, IAuditService auditService)
        {
            _uow = uow;
            _mapper = mapper;
            _auditService = auditService;
            _validator = new CreateKbArticleValidator();
        }

        public async Task<BaseResponse<List<KbArticleSummaryDto>>> GetAllAsync(KbArticleStatus? status, UserRole currentUserRole)
        {
            try
            {
                if (currentUserRole == UserRole.User) status = KbArticleStatus.Published;
                
                IEnumerable<KbArticle> articles;
                if (status.HasValue)
                {
                    articles = await _uow.KbArticles.GetByStatusAsync(status.Value);
                }
                else
                {
                    articles = await _uow.KbArticles.GetAllAsync();
                }

                var list = articles?.ToList() ?? new List<KbArticle>();
                var mapped = _mapper.Map<List<KbArticleSummaryDto>>(list);
                
                return BaseResponse<List<KbArticleSummaryDto>>.Ok(mapped ?? new List<KbArticleSummaryDto>());
            }
            catch (Exception ex)
            {
                return BaseResponse<List<KbArticleSummaryDto>>.Fail($"Error fetching articles: {ex.Message}");
            }
        }

        public async Task<BaseResponse<KbArticleDto>> GetByIdAsync(
            Guid id, Guid currentUserId, UserRole currentUserRole)
        {
            var article = await _uow.KbArticles.GetByIdWithDetailsAsync(id);
            if (article is null) return BaseResponse<KbArticleDto>.Fail("Article not found.");

            if (article.Status == KbArticleStatus.Draft && currentUserRole == UserRole.User)
                return BaseResponse<KbArticleDto>.Fail("Access denied.");

            article.ViewCount++;
            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();

            await _auditService.LogActionAsync("KB_ARTICLE_VIEWED", "KbArticle", article.Id, $"Title: {article.Title}");

            return BaseResponse<KbArticleDto>.Ok(_mapper.Map<KbArticleDto>(article));
        }

        public async Task<BaseResponse<KbArticleDto>> CreateAsync(
            CreateKbArticleCommand command, Guid currentUserId)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<KbArticleDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var article = new KbArticle
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Content = command.Content,
                Tags = command.Tags,
                CategoryId = command.CategoryId,
                AuthorId = currentUserId,
                LastUpdatedById = currentUserId,
                Status = KbArticleStatus.Draft,
                VersionNumber = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId.ToString()
            };
            await _uow.KbArticles.AddAsync(article);
            await _uow.SaveChangesAsync();

            var created = await _uow.KbArticles.GetByIdWithDetailsAsync(article.Id);
            return BaseResponse<KbArticleDto>.Ok(_mapper.Map<KbArticleDto>(created), "Article created as draft.");
        }

        public async Task<BaseResponse<KbArticleDto>> UpdateAsync(
            UpdateKbArticleCommand command, Guid currentUserId, UserRole currentUserRole)
        {
            var article = await _uow.KbArticles.GetByIdWithDetailsAsync(command.Id);
            if (article is null) return BaseResponse<KbArticleDto>.Fail("Article not found.");

            if (currentUserRole == UserRole.Agent &&
                (article.AuthorId != currentUserId || article.Status == KbArticleStatus.Published))
                return BaseResponse<KbArticleDto>.Fail("Agents can only edit their own draft articles.");

            // PRD 9.5: Every save must create a new version entry
            var oldVersion = new KbArticleVersion
            {
                KbArticleId = article.Id,
                VersionNumber = article.VersionNumber,
                Title = article.Title,
                Content = article.Content,
                SavedByUserId = article.LastUpdatedById,
                SavedAt = article.LastModifiedAt ?? article.CreatedAt
            };
            await _uow.KbArticleVersions.AddAsync(oldVersion);

            article.Title = command.Title;
            article.Content = command.Content;
            article.Tags = command.Tags;
            article.CategoryId = command.CategoryId;
            article.LastUpdatedById = currentUserId;
            article.VersionNumber++;
            article.LastModifiedAt = DateTime.UtcNow;
            
            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();

            await _auditService.LogActionAsync("KB_ARTICLE_EDITED", "KbArticle", article.Id, $"New Version: {article.VersionNumber}");

            return BaseResponse<KbArticleDto>.Ok(_mapper.Map<KbArticleDto>(article), "Article updated.");
        }

        public async Task<BaseResponse<object>> PublishAsync(Guid id)
        {
            var article = await _uow.KbArticles.GetByIdAsync(id);
            if (article is null) return BaseResponse<object>.Fail("Article not found.");
            if (article.Status == KbArticleStatus.Published)
                return BaseResponse<object>.Fail("Article is already published.");

            article.Status = KbArticleStatus.Published;
            article.LastModifiedAt = DateTime.UtcNow;
            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();

            await _auditService.LogActionAsync("KB_ARTICLE_PUBLISHED", "KbArticle", article.Id, $"Title: {article.Title}");

            return BaseResponse<object>.Ok(new object(), "Article published.");
        }

        public async Task<BaseResponse<object>> UnpublishAsync(Guid id)
        {
            var article = await _uow.KbArticles.GetByIdAsync(id);
            if (article is null) return BaseResponse<object>.Fail("Article not found.");
            article.Status = KbArticleStatus.Draft;
            article.LastModifiedAt = DateTime.UtcNow;
            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();
            return BaseResponse<object>.Ok(new object(), "Article unpublished.");
        }

        public async Task<BaseResponse<object>> DeleteAsync(Guid id)
        {
            var article = await _uow.KbArticles.GetByIdAsync(id);
            if (article is null) return BaseResponse<object>.Fail("Article not found.");
            _uow.KbArticles.Delete(article);
            await _uow.SaveChangesAsync();
            return BaseResponse<object>.Ok(new object(), "Article deleted.");
        }

        public async Task<BaseResponse<List<KbArticleSummaryDto>>> SearchAsync(string keyword)
        {
            var results = await _uow.KbArticles.SearchAsync(keyword);
            return BaseResponse<List<KbArticleSummaryDto>>.Ok(_mapper.Map<List<KbArticleSummaryDto>>(results));
        }

        public async Task<BaseResponse<List<KbArticleSummaryDto>>> SuggestAsync(string title)
        {
            var results = await _uow.KbArticles.SuggestForTitleAsync(title, 3);
            return BaseResponse<List<KbArticleSummaryDto>>.Ok(_mapper.Map<List<KbArticleSummaryDto>>(results));
        }

        public async Task<BaseResponse<object>> SubmitFeedbackAsync(Guid id, bool isHelpful)
        {
            var article = await _uow.KbArticles.GetByIdAsync(id);
            if (article is null) return BaseResponse<object>.Fail("Article not found.");

            if (isHelpful) article.HelpfulCount++;
            else article.NotHelpfulCount++;

            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();

            await _auditService.LogActionAsync("KB_ARTICLE_FEEDBACK", "KbArticle", article.Id, isHelpful ? "Marked as Helpful" : "Marked as Not Helpful");

            return BaseResponse<object>.Ok(new object(), "Feedback recorded.");
        }

        public async Task<BaseResponse<List<KbArticleVersionDto>>> GetVersionHistoryAsync(Guid articleId)
        {
            var versions = await _uow.KbArticleVersions.GetByArticleIdAsync(articleId);
            return BaseResponse<List<KbArticleVersionDto>>.Ok(_mapper.Map<List<KbArticleVersionDto>>(versions));
        }

        public async Task<BaseResponse<KbArticleDto>> RestoreVersionAsync(Guid articleId, int versionNumber, Guid currentUserId)
        {
            var versions = await _uow.KbArticleVersions.GetByArticleIdAsync(articleId);
            var targetVersion = versions.FirstOrDefault(v => v.VersionNumber == versionNumber);
            if (targetVersion is null) return BaseResponse<KbArticleDto>.Fail("Version not found.");

            var article = await _uow.KbArticles.GetByIdAsync(articleId);
            if (article is null) return BaseResponse<KbArticleDto>.Fail("Article not found.");

            // Archive the CURRENT state as a new version before restoring
            var currentAsVersion = new KbArticleVersion
            {
                KbArticleId = article.Id,
                VersionNumber = article.VersionNumber,
                Title = article.Title,
                Content = article.Content,
                SavedByUserId = article.LastUpdatedById,
                SavedAt = article.LastModifiedAt ?? article.CreatedAt
            };
            await _uow.KbArticleVersions.AddAsync(currentAsVersion);

            // Restore
            article.Title = targetVersion.Title;
            article.Content = targetVersion.Content;
            article.VersionNumber++; // Restoration increments the version
            article.LastUpdatedById = currentUserId;
            article.LastModifiedAt = DateTime.UtcNow;

            _uow.KbArticles.Update(article);
            await _uow.SaveChangesAsync();

            await _auditService.LogActionAsync("KB_ARTICLE_RESTORED", "KbArticle", article.Id, $"Restored from v{versionNumber} to v{article.VersionNumber}");

            return BaseResponse<KbArticleDto>.Ok(_mapper.Map<KbArticleDto>(article), $"Article restored to version {versionNumber}.");
        }
    }
}
