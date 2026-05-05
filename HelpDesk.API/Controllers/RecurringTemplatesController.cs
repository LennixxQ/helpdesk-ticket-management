using HelpDesk.API.Records;
using HelpDesk.Application.Commands.RecurringTemplateCommand;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/recurring-templates")]
    [ApiController]
    public class RecurringTemplatesController : BaseController
    {
        private readonly IRecurringTemplateService _templateService;
        private readonly ILogger<RecurringTemplatesController> _logger;

        public RecurringTemplatesController(
            IRecurringTemplateService templateService,
            ICurrentUserProvider currentUser,
            ILogger<RecurringTemplatesController> logger) : base(currentUser)
        {
            _templateService = templateService;
            _logger = logger;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll() => 
            Ok(await _templateService.GetAllAsync());

        [HttpPost("getById")]
        public async Task<IActionResult> GetById([FromBody] GetByIdRequest dto)
        {
            var result = await _templateService.GetByIdAsync(dto.Id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRecurringTemplateCommand command)
        {
            _logger.LogInformation("Admin {AdminId} creating recurring template: {Name}",_currentUserProvider.GetCurrentUserId(), command.TemplateName);
            var result = await _templateService.CreateAsync(command, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("toggleActive")]
        public async Task<IActionResult> ToggleActive([FromBody] GetByIdRequest dto)
        {
            var result = await _templateService.ToggleActiveAsync(dto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("triggerManual")]
        public async Task<IActionResult> TriggerManual([FromBody] GetByIdRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} manually triggering template {TemplateId}",_currentUserProvider.GetCurrentUserId(), dto.Id);
            var result = await _templateService.TriggerManualAsync(dto.Id, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] GetByIdRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} deleting template {TemplateId}",_currentUserProvider.GetCurrentUserId(), dto.Id);
            var result = await _templateService.DeleteAsync(dto.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
