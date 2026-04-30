using AutoMapper;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.User;
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
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly CreateUserValidator _validator;

        public UserService(IUnitOfWork uow, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _uow = uow;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _validator = new CreateUserValidator();
        }

        public async Task<BaseResponse<UserDto>> CreateUserAsync(CreateUserCommand command)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<UserDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var existing = await _uow.Users.GetByEmailAsync(command.Email);
            if (existing is not null)
                return BaseResponse<UserDto>.Fail("A user with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = command.FullName,
                Email = command.Email,
                UserName = command.Email,
                NormalizedEmail = command.Email.ToUpper(),
                NormalizedUserName = command.Email.ToUpper(),
                Role = command.Role,
                DepartmentId = command.DepartmentId,
                IsActive = true,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "User created successfully.");
        }

        public async Task<BaseResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return BaseResponse<List<UserDto>>.Ok(_mapper.Map<List<UserDto>>(users));
        }

        public async Task<BaseResponse<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null) return BaseResponse<UserDto>.Fail("User not found.");
            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<BaseResponse<UserDto>> UpdateRoleAsync(UpdateUserRoleCommand command)
        {
            var user = await _uow.Users.GetByIdAsync(command.UserId);
            if (user is null) return BaseResponse<UserDto>.Fail("User not found.");

            user.Role = command.NewRole;
            user.LastModifiedAt = DateTime.UtcNow;
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            return BaseResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "Role updated.");
        }

        public async Task<BaseResponse<UserDto>> DeactivateAsync(Guid userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null) return BaseResponse<UserDto>.Fail("User not found.");

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
    }
}