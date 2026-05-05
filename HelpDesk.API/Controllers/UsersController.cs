using HelpDesk.API.Records;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.DTOs.Import;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger, ICurrentUserProvider currentUserProvider) :base(currentUserProvider)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            _logger.LogInformation("Admin {AdminId} creating user: {Email}", _currentUserProvider.GetCurrentUserId(), command.Email);
            var user = await _userService.CreateUserAsync(command);
            return user.Success ? Ok(user) : BadRequest(user);
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var allUsers = await _userService.GetAllUsersAsync();
            return Ok(allUsers);
        }

        [HttpGet("getById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById([FromQuery] GetByIdRequest request)
        {
            var user = await _userService.GetByIdAsync(request.Id);
            return user.Success ? Ok(user) : NotFound(user);
        }

        [HttpPut("UpdateUsersRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateUserRoleCommand request)
        {
            _logger.LogInformation("Admin {AdminId} changing role for user {UserId} to {Role}",_currentUserProvider.GetCurrentUserId(), request.UserId, request.NewRole);
            var command = new UpdateUserRoleCommand { UserId = request.UserId, NewRole = request.NewRole };
            var response = await _userService.UpdateRoleAsync(command);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate([FromBody] GetByIdRequest request)
        {
            _logger.LogInformation("Admin {AdminId} deactivating user {UserId}",_currentUserProvider.GetCurrentUserId(), request.Id);
            var response = await _userService.DeactivateAsync(request.Id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("agents/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveAgents()
        {
            var response = await _userService.GetActiveAgentsAsync();
            return Ok(response);
        }

        // POST api/users/moveDepartment
        [HttpPost("moveDepartment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MoveDepartment([FromBody] MoveDepartmentRequest dto)
        {
            _logger.LogInformation("Admin {AdminId} moving user {UserId} to dept {DeptId}",_currentUserProvider.GetCurrentUserId(), dto.UserId, dto.DepartmentId);
            var result = await _userService.MoveDepartmentAsync(dto.UserId, dto.DepartmentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST api/users/bulkImport
        [HttpPost("bulkImport")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkImport([FromBody] List<BulkImportRowDto> rows)
        {
            _logger.LogInformation("Admin {AdminId} bulk importing {Count} users",_currentUserProvider.GetCurrentUserId(), rows.Count);
            var result = await _userService.BulkImportAsync(rows, _currentUserProvider.GetCurrentUserId());
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
