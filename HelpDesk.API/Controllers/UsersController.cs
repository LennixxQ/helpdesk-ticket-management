using HelpDesk.API.Records;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Import;
using HelpDesk.Application.DTOs.User;
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
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(BaseResponse<bool>.Fail("New password and confirmation do not match."));

            var userId = _currentUserProvider.GetCurrentUserId();
            _logger.LogInformation("User {UserId} changing their password.", userId);

            var response = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("profile-picture")]
        public async Task<IActionResult> UploadProfilePicture([FromBody] UploadProfilePictureRequest request)
        {
            var userId = _currentUserProvider.GetCurrentUserId().ToString();
            _logger.LogInformation("User {UserId} uploading profile picture.", userId);

            try
            {
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatars");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                var imagePath = Path.Combine(directoryPath, $"{userId}.avatar");
                await System.IO.File.WriteAllTextAsync(imagePath, request.Base64Image);

                var positionPath = Path.Combine(directoryPath, $"{userId}.position");
                await System.IO.File.WriteAllTextAsync(positionPath, request.Position);

                return Ok(BaseResponse<bool>.Ok(true, "Profile picture uploaded successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
                return StatusCode(500, BaseResponse<bool>.Fail("An error occurred while uploading profile picture."));
            }
        }

        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetProfilePicture([FromQuery] string? userId = null)
        {
            var targetUserId = userId;
            if (string.IsNullOrEmpty(targetUserId))
            {
                try
                {
                    targetUserId = _currentUserProvider.GetCurrentUserId().ToString();
                }
                catch
                {
                    return BadRequest(BaseResponse<ProfilePictureResponse>.Fail("User ID is required or user must be authenticated."));
                }
            }

            try
            {
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatars");
                var imagePath = Path.Combine(directoryPath, $"{targetUserId}.avatar");
                var positionPath = Path.Combine(directoryPath, $"{targetUserId}.position");

                string? base64Image = null;
                string position = "50% 50%";

                if (System.IO.File.Exists(imagePath))
                {
                    base64Image = await System.IO.File.ReadAllTextAsync(imagePath);
                }

                if (System.IO.File.Exists(positionPath))
                {
                    position = await System.IO.File.ReadAllTextAsync(positionPath);
                }

                return Ok(BaseResponse<ProfilePictureResponse>.Ok(new ProfilePictureResponse(base64Image, position),"Profile picture retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile picture for user {UserId}", targetUserId);
                return StatusCode(500, BaseResponse<ProfilePictureResponse>.Fail("An error occurred while retrieving profile picture."));
            }
        }
    }
}
