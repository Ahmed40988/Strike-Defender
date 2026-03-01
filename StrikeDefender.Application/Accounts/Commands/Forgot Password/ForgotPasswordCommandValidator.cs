namespace StrikeDefender.Application.Accounts.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty()
               .EmailAddress();

        }

    }
}
