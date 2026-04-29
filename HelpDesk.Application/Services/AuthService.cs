using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork)
        {
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<string>> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BaseResponse<string>.Fail("Email and password are required.");

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user is null || !user.IsActive)
                return BaseResponse<string>.Fail("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
            if (result == PasswordVerificationResult.Failed)
                return BaseResponse<string>.Fail("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(user);
            return BaseResponse<string>.Ok(token, "Login successful.");
        }
    }
}
