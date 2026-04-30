using HelpDesk.Application.DTOs.Department;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetById([FromBody] DepartmentIdRequest request)
        {
            var result = await _departmentService.GetByIdAsync(request.DepartmentId);
            return Ok(result);
        }

        [HttpPost("getDeptSummary")]
        public async Task<IActionResult> GetSummary([FromBody] DepartmentIdRequest request)
        {
            var result = await _departmentService.GetSummaryAsync(request.DepartmentId);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            _logger.LogInformation("Derpartment Created: {Name}", dto.Name);
            var result = await _departmentService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateDepartmentWithIdDto dto)
        {
            _logger.LogInformation("Department {Name} Details Updated", dto.Name);
            var result = await _departmentService.UpdateAsync(dto.DepartmentId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> Deactivate([FromBody] DepartmentIdRequest request)
        {
            var result = await _departmentService.DeactivateAsync(request.DepartmentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("assignHead")]
        public async Task<IActionResult> AssignHead([FromBody] AssignDepartmentHeadRequest request)
        {
            _logger.LogInformation("Department {Name} Head Assigned {ID}", request.DepartmentId, request.UserId);
            var result = await _departmentService.AssignHeadAsync(request.DepartmentId, request.UserId);
            return Ok(result);
        }

        public record DepartmentIdRequest(Guid DepartmentId);
        public record AssignDepartmentHeadRequest(Guid DepartmentId, Guid UserId);
    }
}
