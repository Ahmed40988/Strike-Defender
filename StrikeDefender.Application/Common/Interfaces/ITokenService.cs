namespace StrikeDefender.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<(string Token, int ExpiresIn)> GenerateTokenAsync(
            Guid domainUserId,
            string email,
            string userName,
            IEnumerable<string> roles
        );

        Guid? ValidateToken(string token);
    }
}
