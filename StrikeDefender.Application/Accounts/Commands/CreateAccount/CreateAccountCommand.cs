namespace StrikeDefender.Application.Accounts.Commands.CreateAccount
{
    public record CreateAccountCommand(string Email, string Passsword) : IRequest<ErrorOr<Success>>;
}
