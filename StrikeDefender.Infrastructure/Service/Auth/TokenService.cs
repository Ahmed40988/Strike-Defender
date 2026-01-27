using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StrikeDefender.Application.Common.Interfaces;

namespace StrikeDefender.Infrastructure.Service.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<(string Token, int ExpiresIn)> GenerateTokenAsync(
            Guid domainUserId,
            string email,
            string userName,
            IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim("domainUserId", domainUserId.ToString()),

                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)
            );

            var expiresInMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return Task.FromResult((
                new JwtSecurityTokenHandler().WriteToken(token),
                expiresInMinutes * 60
            ));
        }

        public Guid? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var domainUserIdClaim =
                    jwtToken.Claims.FirstOrDefault(x => x.Type == "domainUserId");

                return domainUserIdClaim is null
                    ? null
                    : Guid.Parse(domainUserIdClaim.Value);
            }
            catch
            {
                return null;
            }
        }
    }
}
