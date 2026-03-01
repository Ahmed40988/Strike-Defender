namespace StrikeDefender.Application.Accounts.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Email,
        string Token,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<ErrorOr<Success>>;
}
