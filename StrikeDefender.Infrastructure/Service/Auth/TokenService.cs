using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StrikeDefender.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Web.Infrastructure.Service.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<(string Token, int expiresIn)> GenerateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {

            _logger.LogInformation(
    "SIGN KEY (GEN): {Key}",
    Convert.ToBase64String(
        Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!.Trim())
    )
);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var Roles = await userManager.GetRolesAsync(user);

            foreach (var Role in Roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            var authKeyInByets = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!.Trim()));

            var JwtObject = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: userClaims,
               expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(authKeyInByets, SecurityAlgorithms.HmacSha256
)
            );
            var expiresIn = int.Parse(_configuration["JWT:ExpiryMinutes"]);

            return (token: new JwtSecurityTokenHandler().WriteToken(JwtObject), expiresIn: expiresIn * 60);
        }

        public string? ValidateAccessToken(string token)
        {
            return Validate(token, validateLifetime: true);
        }

        public string? GetUserIdFromExpiredToken(string token)
        {
            return Validate(token, validateLifetime: false);
        }


        private string? Validate(string token, bool validateLifetime)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!.Trim())
            );

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal.Claims
                .First(x => x.Type == ClaimTypes.NameIdentifier)
                .Value;
        }

    }
}
