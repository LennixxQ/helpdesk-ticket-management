using AutoMapper;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<BaseResponse<UserDto>> CreateUserAsync(CreateUserCommand command)
        {
            var validator = new CreateUserValidator();
            var result = await validator.ValidateAsync(command);
            if (!result.IsValid)
                return BaseResponse<UserDto>.Fail(result.Errors.Select(e => e.ErrorMessage).ToList());

            var existing = await _userManager.FindByEmailAsync(command.Email);
            if(existing is not null)
                return BaseResponse<UserDto>.Fail("Email already in use.");

            var user = _mapper.Map<User>(command);
            user.CreatedAt = DateTime.UtcNow;

            var createResult = await _userManager.CreateAsync(user, command.Password);
            if (!createResult.Succeeded)
                return BaseResponse<UserDto>.Fail(createResult.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRoleAsync(user, command.Role.ToString());

            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "User created successfully.");

        }

        public async Task<BaseResponse<UserDto>> DeactivateAsync(Guid userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null)
                return BaseResponse<UserDto>.Fail("User not found.");

            user.IsActive = false;
            user.LastModifiedAt = DateTime.UtcNow;

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "User deactivated.");
        }

        public async Task<BaseResponse<List<UserDto>>> GetActiveAgentsAsync()
        {
            var agents = await _uow.Users.GetActiveAgentsAsync();
            return BaseResponse<List<UserDto>>.Ok(_mapper.Map<List<UserDto>>(agents));
        }

        public async Task<BaseResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return BaseResponse<List<UserDto>>.Ok(_mapper.Map<List<UserDto>>(users));
        }

        public async Task<BaseResponse<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null)
                return BaseResponse<UserDto>.Fail("User not found.");
            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<BaseResponse<UserDto>> UpdateRoleAsync(UpdateUserRoleCommand command)
        {
            var user = await _uow.Users.GetByIdAsync(command.UserId);
            if (user is null)
                return BaseResponse<UserDto>.Fail("User not found.");

            var oldRole = user.Role.ToString();
            user.Role = command.NewRole;
            user.LastModifiedAt = DateTime.UtcNow;

            await _userManager.RemoveFromRoleAsync(user, oldRole);
            await _userManager.AddToRoleAsync(user, command.NewRole.ToString());

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "Role updated.");

        }
    }
}
