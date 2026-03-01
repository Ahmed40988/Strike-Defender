using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<(string Token, int expiresIn)> GenerateTokenAsync(AppUser user, UserManager<AppUser> userManager);
        string? ValidateAccessToken(string token);
        string? GetUserIdFromExpiredToken(string token);

    }
}
