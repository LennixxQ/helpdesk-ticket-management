using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelpDesk.Infrastructure.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            return CreateToken(user, true);
        }

        public string GenerateMfaToken(User user)
        {
            return CreateToken(user, false);
        }

        private string CreateToken(User user, bool isFinal)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Name, user.FullName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("SecurityStamp", user.SecurityStamp ?? string.Empty),
            };

            if (isFinal)
            {
                claims.Add(new(ClaimTypes.Role, user.Role.ToString()));
            }
            else
            {
                claims.Add(new("mfa_pending", "true"));
            }

            var expiry = isFinal 
                ? DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!))
                : DateTime.UtcNow.AddMinutes(15); // Increased to 15 mins for easier setup

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
