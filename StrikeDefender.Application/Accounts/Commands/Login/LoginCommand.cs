using StrikeDefender.Application.Accounts.AccountDTO;

namespace StrikeDefender.Application.Accounts.Commands.Login
{
    public record LoginCommand(string Email, string Password)
        : IRequest<ErrorOr<TokenDTO>>;
}
