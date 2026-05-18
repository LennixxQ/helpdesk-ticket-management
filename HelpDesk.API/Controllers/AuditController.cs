using HelpDesk.API.Records;
using HelpDesk.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AuditController> _logger;

        public AuditController(ICurrentUserProvider currentUserProvider, IUnitOfWork uow, ILogger<AuditController> logger) : base(currentUserProvider)
        {
            _uow = uow;
            _logger = logger;
        }

        [HttpPost("getAll")]
        public async Task<IActionResult> GetAll([FromBody] AuditFilterRequest dto)
        {
            _logger.LogInformation("Admin {Id} querying audit log", _currentUserProvider.GetCurrentUserId());
            var (items, total) = await _uow.AuditLogs.GetPagedAsync(dto.From, dto.To, dto.Actor, dto.Action, dto.EntityType, dto.Page, dto.PageSize);

            return Ok(new
            {
                success = true,
                message = "Audit logs retrieved successfully",
                data = new
                {
                    items,
                    totalCount = total,
                    page = dto.Page,
                    pageSize = dto.PageSize
                }
            });
        }

        [HttpPost("getByEntity")]
        public async Task<IActionResult> GetByEntity([FromBody] GetByIdRequest dto)
        {
            var logs = await _uow.AuditLogs.GetByEntityIdAsync(dto.Id);
            return Ok(new { success = true, data = logs });
        }
    }
}
