using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(UserManager<User> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<BaseResponse<string>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return BaseResponse<string>.Fail("Invalid Credentials");

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if(!isValid)
                return BaseResponse<string>.Fail("Invalid Credentials");

            var token = _jwtTokenService.GenerateToken(user);
            return BaseResponse<string>.Ok(token, "Login Successful!");
        }
    }
}
