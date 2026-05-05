using AutoMapper;
using HelpDesk.Application.Commands.UserCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Import;
using HelpDesk.Application.DTOs.User;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly INotificationService _notificationService;
        private readonly CreateUserValidator _validator;
        private readonly BulkImportRowValidator _bulkValidator;

        public UserService(IUnitOfWork uow,IMapper mapper,IPasswordHasher<User> passwordHasher, INotificationService notificationService)
        {
            _uow = uow;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _notificationService = notificationService;
            _validator = new CreateUserValidator();
            _bulkValidator = new BulkImportRowValidator();
        }

        public async Task<BaseResponse<UserDto>> CreateUserAsync(CreateUserCommand command)
        {
            var validation = await _validator.ValidateAsync(command);
            if (!validation.IsValid)
                return BaseResponse<UserDto>.Fail("Validation failed.",validation.Errors.Select(e => e.ErrorMessage).ToList());

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
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system",
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

        public async Task<BaseResponse<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null) return BaseResponse<bool>.Fail("User not found.");

            // 1. Verify Current Password
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
                return BaseResponse<bool>.Fail("Incorrect current password.");

            // 2. Hash New Password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            // 3. Update Security Stamp (This logs out all active sessions!)
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.LastModifiedAt = DateTime.UtcNow;

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            // 4. Send Email Notification
            await _notificationService.SendPasswordChangedAsync(user);

            return BaseResponse<bool>.Ok(true, "Password changed successfully. Please login again.");
        }

        public async Task<BaseResponse<List<UserDto>>> GetActiveAgentsAsync()
        {
            var agents = await _uow.Users.GetActiveAgentsAsync();
            return BaseResponse<List<UserDto>>.Ok(_mapper.Map<List<UserDto>>(agents));
        }

        public async Task<BaseResponse<object>> MoveDepartmentAsync(Guid userId, Guid departmentId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user is null) return BaseResponse<object>.Fail("User not found.");

            var dept = await _uow.Departments.GetByIdAsync(departmentId);
            if (dept is null || !dept.IsActive)
                return BaseResponse<object>.Fail("Department not found or inactive.");

            user.DepartmentId = departmentId;
            user.LastModifiedAt = DateTime.UtcNow;
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            return BaseResponse<object>.Ok(new object(),
                $"User moved to department '{dept.Name}' successfully.");
        }

        public async Task<BaseResponse<BulkImportResultDto>> BulkImportAsync(
            List<BulkImportRowDto> rows, Guid adminId)
        {
            var result = new BulkImportResultDto();
            var errors = new List<string>();

            for (int i = 0; i < rows.Count; i++)
            {
                var validation = await _bulkValidator.ValidateAsync(rows[i]);
                if (!validation.IsValid)
                    foreach (var err in validation.Errors)errors.Add($"Row {i + 1}: {err.ErrorMessage}");
            }

            if (errors.Any())
            {
                result.Errors = errors;
                return BaseResponse<BulkImportResultDto>.Fail(
                    "Validation failed. No accounts created.", errors);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var existing = await _uow.Users.GetByEmailAsync(row.Email);
                if (existing is not null)
                {
                    result.RowsSkipped++;
                    errors.Add($"Row {i + 1}: Email '{row.Email}' already exists — skipped.");
                    continue;
                }

                var role = Enum.TryParse<UserRole>(row.Role, out var r) ? r : UserRole.User;
                var dept = await _uow.Departments.GetByNameAsync(row.Department);
                var tempPass = GenerateTempPassword();

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = row.Name,
                    Email = row.Email,
                    UserName = row.Email,
                    NormalizedEmail = row.Email.ToUpper(),
                    NormalizedUserName = row.Email.ToUpper(),
                    Role = role,
                    DepartmentId = dept?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminId.ToString(),
                    EmailConfirmed = true
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, tempPass);

                await _uow.Users.AddAsync(user);
                result.AccountsCreated++;
            }

            await _uow.SaveChangesAsync();
            result.Errors = errors;

            return BaseResponse<BulkImportResultDto>.Ok(result,
                $"{result.AccountsCreated} accounts created, {result.RowsSkipped} skipped.");
        }

        private static string GenerateTempPassword()
        {
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789!@#$";
            var rng = new Random();
            return new string(Enumerable.Range(0, 12).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }
}