namespace StrikeDefender.Application.Accounts.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string OTP)
        : IRequest<ErrorOr<Success>>;
}
