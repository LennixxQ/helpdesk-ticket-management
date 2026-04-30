using HelpDesk.Application.Commands.AuthCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly LoginValidator _validator;

        public AuthService(IUnitOfWork uow, IJwtTokenService jwtService, IPasswordHasher<User> passwordHasher)
        {
            _uow = uow;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _validator = new LoginValidator();
        }

        public async Task<BaseResponse<string>> LoginAsync(LoginRequest request)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return BaseResponse<string>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await _uow.Users.GetByEmailAsync(request.Email);
            if (user is null || !user.IsActive)
                return BaseResponse<string>.Fail("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return BaseResponse<string>.Fail("Invalid email or password.");

            var token = _jwtService.GenerateToken(user);
            return BaseResponse<string>.Ok(token, "Login successful.");
        }
    }
}