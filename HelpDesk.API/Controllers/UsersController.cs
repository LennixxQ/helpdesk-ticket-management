using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            _logger.LogInformation("Admin Creating User with Email: {Email}", command.Email);
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
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user.Success ? Ok(user) : NotFound(user);
        }

        [HttpPut("UpdateUsersRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var command = new UpdateUserRoleCommand { UserId = id, NewRole = request.NewRole };
            var response = await _userService.UpdateRoleAsync(command);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var response = await _userService.DeactivateAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("agents/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveAgents()
        {
            var response = await _userService.GetActiveAgentsAsync();
            return Ok(response);
        }

        public record UpdateRoleRequest(UserRole NewRole);
    }
}
