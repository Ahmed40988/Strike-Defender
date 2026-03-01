namespace StrikeDefender.Application.Accounts.Commands.VerifyForgotPasswordOtp
{
    public record VerifyForgotPasswordOtpCommand(string Email, string OTP)
        : IRequest<ErrorOr<string>>;
}
