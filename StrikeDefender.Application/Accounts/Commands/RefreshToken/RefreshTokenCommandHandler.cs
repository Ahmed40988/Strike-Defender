using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using StrikeDefender.Application.Accounts.AccountDTO;
using StrikeDefender.Domain.Users;
using RefreshTokenEntity = StrikeDefender.Domain.RefreshTokens.RefreshToken;


namespace StrikeDefender.Application.Accounts.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
          ILogger<RefreshTokenCommandHandler> logger,
        ITokenService tokenService,
        UserManager<AppUser> userManager
    ) : IRequestHandler<RefreshTokenCommand, ErrorOr<TokenDTO>>
    {
        private readonly ILogger _logger = logger;
        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<ErrorOr<TokenDTO>> Handle(
            RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("TOKEN VALUE: {Token}", command.Token);

            var userId = _tokenService.GetUserIdFromExpiredToken(command.Token);
            if (userId is null)
                return Error.Validation("Auth.InvalidJwt", "Invalid JWT token");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.Validation("Auth.InvalidCredentials", "Invalid credentials");

            var oldToken = user.RefreshTokens
       .SingleOrDefault(x => x.Token == command.RefreshToken && x.IsActive);

            if (oldToken is null)
                return Error.Validation("Auth.InvalidRefreshToken", "Invalid refresh token");

            oldToken.Revoke();

            var jwt = await _tokenService.GenerateTokenAsync(user, _userManager);

            var newRefreshTokenValue = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            var newRefreshToken = RefreshTokenEntity.Create(
                newRefreshTokenValue,
                refreshTokenExpiration
            );

            user.AddrefreshTokens(newRefreshToken);

            await _userManager.UpdateAsync(user);

            return new TokenDTO
            {
                UserId = user.Id,
                Token = jwt.Token,
                expiresIn = jwt.expiresIn,
                RefreshToken = newRefreshTokenValue,
                RefreshTokenExpiration = refreshTokenExpiration
            };

        }
    }
}
