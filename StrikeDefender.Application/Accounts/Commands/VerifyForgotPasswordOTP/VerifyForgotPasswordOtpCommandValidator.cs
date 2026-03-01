using StrikeDefender.Application.Accounts.Commands.VerifyForgotPasswordOtp;

namespace StrikeDefender.Application.Accounts.Commands.VerifyForgotPasswordOTP
{
    public class VerifyForgotPasswordOtpCommandValidator : AbstractValidator<VerifyForgotPasswordOtpCommand>
    {
        public VerifyForgotPasswordOtpCommandValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty()
              .EmailAddress();

            RuleFor(x => x.OTP)
            .NotEmpty().WithMessage("OTP is required")
            .Matches(@"^\d{6}$")
            .WithMessage("OTP must be exactly 6 digits");

        }
    }
}
