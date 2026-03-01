namespace StrikeDefender.Application.Accounts.Commands.ResendConfirmationEmail
{
    public record ResendConfirmationEmailCommand(string Email)
        : IRequest<ErrorOr<Success>>;
}


