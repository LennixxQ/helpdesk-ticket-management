using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Infrastructure.Identity
{
    public class IdentityService :IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Success, string[] Errors)> ChangeRoleAsync(User user, UserRole newRole)
        {
            var oldRole = user.Role.ToString();
            var newRoleStr = newRole.ToString();

            await _userManager.RemoveFromRoleAsync(user, oldRole);
            var result = await _userManager.AddToRoleAsync(user, newRoleStr);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return (true, []);
        }

        public async Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password, CancellationToken ct)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            await _userManager.AddToRoleAsync(user, user.Role.ToString());
            return (true, []);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return null;

            var valid = await _userManager.CheckPasswordAsync(user, password);
            return valid ? user : null;
        }
    }
}
