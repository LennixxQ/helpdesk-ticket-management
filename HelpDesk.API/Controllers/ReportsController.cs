using HelpDesk.API.Records;
using HelpDesk.Application.DTOs.Report;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService,ICurrentUserProvider currentUser,ILogger<ReportsController> logger) : base(currentUser)
        {
            _reportService = reportService;
            _logger = logger;
        }

        // POST api/reports/ticketVolume
        [HttpPost("ticketVolume")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TicketVolume([FromBody] ReportFilterDto filter)
        {
            _logger.LogInformation("Admin {Id} generating ticket volume report",_currentUserProvider.GetCurrentUserId());
            return Ok(await _reportService.GetTicketVolumeReportAsync(filter));
        }

        // POST api/reports/agentPerformance
        [HttpPost("agentPerformance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AgentPerformance([FromBody] AgentReportRequest dto)
            => Ok(await _reportService.GetAgentPerformanceReportAsync(dto.AgentId, dto.Filter));

        // POST api/reports/slaCompliance
        [HttpPost("slaCompliance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SlaCompliance([FromBody] ReportFilterDto filter)
            => Ok(await _reportService.GetSlaComplianceReportAsync(filter));

        // GET api/reports/topAgent
        [HttpGet("topAgent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TopAgent([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var filter = new ReportFilterDto
            {
                From = from ?? DateTime.UtcNow.AddDays(-30),
                To = to ?? DateTime.UtcNow
            };
            _logger.LogInformation("Admin {Id} getting top agent report", _currentUserProvider.GetCurrentUserId());
            return Ok(await _reportService.GetTopAgentReportAsync(filter));
        }
        [HttpGet("agentSelf")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> AgentSelf()
        {
            var result = await _reportService.GetAgentSelfServiceReportAsync(_currentUserProvider.GetCurrentUserId());
            return Ok(result);
        }

        // POST api/reports/exportTicketsCsv
        [HttpPost("exportTicketsCsv")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportTicketsCsv([FromBody] ReportFilterDto filter)
        {
            _logger.LogInformation("Admin {Id} exporting tickets CSV",_currentUserProvider.GetCurrentUserId());
            var bytes = await _reportService.ExportTicketsCsvAsync(filter);
            return File(bytes, "text/csv",
                $"tickets_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
        }

        // POST api/reports/exportTicketsPdf
        [HttpPost("exportTicketsPdf")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportTicketsPdf([FromBody] ReportFilterDto filter)
        {
            _logger.LogInformation("Admin {Id} exporting tickets PDF",_currentUserProvider.GetCurrentUserId());
            var bytes = await _reportService.ExportTicketsPdfAsync(filter);
            return File(bytes, "application/pdf",
                $"tickets_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf");
        }

        // POST api/reports/exportAuditCsv
        [HttpPost("exportAuditCsv")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAuditCsv([FromBody] AuditExportRequest dto)
        {
            _logger.LogInformation("Admin {Id} exporting audit CSV {From} to {To}",_currentUserProvider.GetCurrentUserId(), dto.From, dto.To);
            var bytes = await _reportService.ExportAuditLogCsvAsync(dto.From, dto.To);
            return File(bytes, "text/csv",
                $"audit_{dto.From:yyyyMMdd}_{dto.To:yyyyMMdd}.csv");
        }
    }
}
