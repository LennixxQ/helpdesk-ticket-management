using HelpDesk.Application.Commands.KbCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.KbArticle;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IKbArticleService
    {
        Task<BaseResponse<List<KbArticleSummaryDto>>> GetAllAsync(KbArticleStatus? status, UserRole currentUserRole);
        Task<BaseResponse<KbArticleDto>> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<KbArticleDto>> CreateAsync(CreateKbArticleCommand command, Guid currentUserId);
        Task<BaseResponse<KbArticleDto>> UpdateAsync(UpdateKbArticleCommand command, Guid currentUserId, UserRole currentUserRole);
        Task<BaseResponse<object>> PublishAsync(Guid id);
        Task<BaseResponse<object>> UnpublishAsync(Guid id);
        Task<BaseResponse<object>> DeleteAsync(Guid id);
        Task<BaseResponse<List<KbArticleSummaryDto>>> SearchAsync(string keyword);
        Task<BaseResponse<List<KbArticleSummaryDto>>> SuggestAsync(string title);
        Task<BaseResponse<object>> SubmitFeedbackAsync(Guid id, bool isHelpful);
    }
}
