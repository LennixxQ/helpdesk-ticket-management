using HelpDesk.API.Records;
using HelpDesk.Application.Commands.DepartmentCommand;
using HelpDesk.Application.DTOs.Department;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger, ICurrentUserProvider currentUserProvider) : base(currentUserProvider)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("getDeptById")]
        public async Task<IActionResult> GetById([FromBody] GetByIdRequest request)
        {
            var result = await _departmentService.GetByIdAsync(request.Id);
            return Ok(result);
        }

        [HttpPost("getDeptSummary")]
        public async Task<IActionResult> GetSummary([FromBody] GetByIdRequest request)
        {
            var result = await _departmentService.GetSummaryAsync(request.Id);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            _logger.LogInformation("Admin {AdminId} creating department: {Name}",_currentUserProvider.GetCurrentUserId(), command.Name);
            var result = await _departmentService.CreateAsync(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateDepartmentCommand command)
        {
            var result = await _departmentService.UpdateAsync(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> Deactivate([FromBody] GetByIdRequest request)
        {
            _logger.LogInformation("Admin {AdminId} deactivating department {DeptId}",_currentUserProvider.GetCurrentUserId(), request.Id);
            var result = await _departmentService.DeactivateAsync(request.Id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("assignHead")]
        public async Task<IActionResult> AssignHead([FromBody] AssignHeadRequest request)
        {
            _logger.LogInformation("Admin {AdminId} assigning head {UserId} to dept {DeptId}",_currentUserProvider.GetCurrentUserId(), request.UserId, request.DepartmentId);
            var result = await _departmentService.AssignHeadAsync(request.DepartmentId, request.UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
