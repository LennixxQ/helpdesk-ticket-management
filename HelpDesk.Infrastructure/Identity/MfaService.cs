using HelpDesk.Application.Common;
using HelpDesk.Application.DTOs.Auth;
using HelpDesk.Application.DTOs.MFA;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OtpNet;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;

namespace HelpDesk.Infrastructure.Identity
{
    public class MfaService : IMfaService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenService _jwtService;
        private readonly IConfiguration _config;
        private readonly ILogger<MfaService> _logger;

        public MfaService(IUnitOfWork uow, IJwtTokenService jwtService, IConfiguration config, ILogger<MfaService> logger)
        {
            _uow = uow;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
        }

        public async Task<BaseResponse<MfaSetupDto>> GetMfaSetupAsync(Guid userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return BaseResponse<MfaSetupDto>.Fail("User not found.");

            // Generate a 160-bit secret key (32 characters in Base32)
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);

            // Save the secret temporarily until verified
            user.MfaSecretKey = base32Secret;
            user.IsMfaEnabled = false;
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            var issuer = _config["MfaSettings:Issuer"] ?? "HelpDesk";
            var account = user.Email;
            var qrCodeUrl = $"otpauth://totp/{issuer}:{account}?secret={base32Secret}&issuer={issuer}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            var base64Image = Convert.ToBase64String(qrCodeImage);

            return BaseResponse<MfaSetupDto>.Ok(new MfaSetupDto
            {
                SecretKey = base32Secret,
                QrCodeDataUri = $"data:image/png;base64,{base64Image}"
            });
        }

        public async Task<BaseResponse<bool>> VerifyAndEnableMfaAsync(string? jwtToken, string code, Guid? userId = null)
        {
            Guid finalUserId;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwtToken))
                    return BaseResponse<bool>.Fail("Invalid session token.");

                var readToken = handler.ReadJwtToken(jwtToken);
                if (string.IsNullOrEmpty(readToken.Subject))
                    return BaseResponse<bool>.Fail("Invalid session token claims.");

                finalUserId = Guid.Parse(readToken.Subject);
            }
            else if (userId.HasValue)
            {
                finalUserId = userId.Value;
            }
            else
            {
                return BaseResponse<bool>.Fail("User identification missing.");
            }

            var user = await _uow.Users.GetByIdAsync(finalUserId);
            if (user == null) return BaseResponse<bool>.Fail("User not found.");

            if (string.IsNullOrEmpty(user.MfaSecretKey))
                return BaseResponse<bool>.Fail("MFA not initialized. Please call setup first.");

            if (VerifyCode(user.MfaSecretKey, code))
            {
                user.IsMfaEnabled = true;
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();
                return BaseResponse<bool>.Ok(true, "MFA enabled successfully.");
            }

            return BaseResponse<bool>.Fail("Invalid verification code.");
        }

        public async Task<BaseResponse<bool>> DisableMfaAsync(Guid userId, string code)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return BaseResponse<bool>.Fail("User not found.");

            if (!user.IsMfaEnabled)
                return BaseResponse<bool>.Fail("MFA is not enabled.");

            if (VerifyCode(user.MfaSecretKey!, code))
            {
                user.IsMfaEnabled = false;
                user.MfaSecretKey = null; // Clear the secret for security
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();
                return BaseResponse<bool>.Ok(true, "MFA disabled successfully.");
            }

            return BaseResponse<bool>.Fail("Invalid verification code.");
        }

        public async Task<BaseResponse<LoginResponse>> VerifyLoginCodeAsync(string jwtToken, string code)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(jwtToken))
                return BaseResponse<LoginResponse>.Fail("Invalid session token.");

            var readToken = handler.ReadJwtToken(jwtToken);
            var userIdClaim = readToken.Subject;
            
            if (string.IsNullOrEmpty(userIdClaim))
                return BaseResponse<LoginResponse>.Fail("Invalid session token claims.");

            var userId = Guid.Parse(userIdClaim);
            _logger.LogInformation("MFA Login Verification: Token UserId={UserId}", userId);
            
            var user = await _uow.Users.GetByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("MFA Login Verification: User {UserId} not found in database.", userId);
                return BaseResponse<LoginResponse>.Fail("User not found or inactive.");
            }

            if (!user.IsActive)
            {
                 _logger.LogWarning("MFA Login Verification: User {UserId} is inactive.", userId);
                 return BaseResponse<LoginResponse>.Fail("User not found or inactive.");
            }

            _logger.LogInformation("MFA Login Verification: User {UserId} found. IsMfaEnabled={IsMfaEnabled}, HasSecret={HasSecret}", 
                userId, user.IsMfaEnabled, !string.IsNullOrEmpty(user.MfaSecretKey));

            if (!user.IsMfaEnabled || string.IsNullOrEmpty(user.MfaSecretKey))
            {
                return BaseResponse<LoginResponse>.Fail("MFA is not enabled for this user.");
            }

            if (VerifyCode(user.MfaSecretKey, code))
            {
                var token = _jwtService.GenerateToken(user);
                return BaseResponse<LoginResponse>.Ok(new LoginResponse
                {
                    Token = token
                }, "MFA verification successful.");
            }

            return BaseResponse<LoginResponse>.Fail("Invalid MFA code.");
        }

        public string GenerateMfaSessionToken(Guid userId)
        {
            // We need the User object to generate the token
            // This is a bit inefficient if we don't have it, but usually we do during login.
            // Since this is a helper, I'll let the AuthService call _jwtService.GenerateMfaToken(user)
            return string.Empty; // Not used as AuthService calls JwtService directly
        }

        private bool VerifyCode(string base32Secret, string code)
        {
            try
            {
                var secretKey = Base32Encoding.ToBytes(base32Secret);
                var totp = new Totp(secretKey);
                return totp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(1, 1));
            }
            catch
            {
                return false;
            }
        }
    }
}
