using HelpDesk.API.Records;
using HelpDesk.Application.Commands.KbCommand;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeBaseController : BaseController
    {
        private readonly IKbArticleService _kbService;
        private readonly ILogger<KnowledgeBaseController> _logger;

        public KnowledgeBaseController(IKbArticleService kbService, ILogger<KnowledgeBaseController> logger, ICurrentUserProvider currentUser) : base(currentUser)
        {
            _kbService = kbService;
            _logger = logger;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] KbArticleStatus? status) => 
            Ok(await _kbService.GetAllAsync(status, _currentUserProvider.GetCurrentUserRole()));

        [HttpPost("getById")]
        public async Task<IActionResult> GetById([FromBody] GetByIdRequest dto)
        {
            var result = await _kbService.GetByIdAsync(dto.Id,_currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) =>
            Ok(await _kbService.SearchAsync(keyword));

        [HttpGet("suggest")]
        public async Task<IActionResult> Suggest([FromQuery] string title) =>
            Ok(await _kbService.SuggestAsync(title));

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Create([FromBody] CreateKbArticleCommand command)
        {
            _logger.LogInformation("User {UserId} creating KB article: {Title}",_currentUserProvider.GetCurrentUserId(), command.Title);
            var result = await _kbService.CreateAsync(command, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Update([FromBody] UpdateKbArticleCommand command)
        {
            var result = await _kbService.UpdateAsync(command,_currentUserProvider.GetCurrentUserId(), _currentUserProvider.GetCurrentUserRole());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("publish")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Publish([FromBody] GetByIdRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} publishing KB article {ArticleId}",_currentUserProvider.GetCurrentUserId(), dto.Id);
            var result = await _kbService.PublishAsync(dto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("unpublish")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unpublish([FromBody] GetByIdRequest dto)
        {
            var result = await _kbService.UnpublishAsync(dto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromBody] GetByIdRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} deleting KB article {ArticleId}",_currentUserProvider.GetCurrentUserId(), dto.Id);
            var result = await _kbService.DeleteAsync(dto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("submitFeedback")]
        public async Task<IActionResult> Feedback([FromBody] FeedbackRequest dto) =>
            Ok(await _kbService.SubmitFeedbackAsync(dto.ArticleId, dto.IsHelpful));
    }
}
