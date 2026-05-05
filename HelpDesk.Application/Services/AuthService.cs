using HelpDesk.Application.Commands.AuthCommand;
using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Auth;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Validators;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace HelpDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;
        private readonly LoginValidator _validator;

        public AuthService(IUnitOfWork uow, IJwtTokenService jwtService, IPasswordHasher<User> passwordHasher, IConfiguration config)
        {
            _uow = uow;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _config = config;
            _validator = new LoginValidator();
        }

        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return BaseResponse<LoginResponse>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await _uow.Users.GetByEmailAsync(request.Email);
            if (user is null || !user.IsActive)
                return BaseResponse<LoginResponse>.Fail("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return BaseResponse<LoginResponse>.Fail("Invalid email or password.");

            if (user.IsMfaEnabled)
            {
                var mfaToken = _jwtService.GenerateMfaToken(user);
                return BaseResponse<LoginResponse>.Ok(new LoginResponse
                {
                    MfaSessionToken = mfaToken,
                    RequiresSetup = false
                }, "MFA verification required.");
            }

            var token = _jwtService.GenerateToken(user);
            return BaseResponse<LoginResponse>.Ok(new LoginResponse
            {
                Token = token
            }, "Login successful.");
        }
    }
}