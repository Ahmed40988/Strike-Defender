using StrikeDefender.Application.Accounts.AccountDTO;

namespace StrikeDefender.Application.Accounts.Commands.RefreshToken
{
    public record RefreshTokenCommand(string Token, string RefreshToken)
        : IRequest<ErrorOr<TokenDTO>>;
}
