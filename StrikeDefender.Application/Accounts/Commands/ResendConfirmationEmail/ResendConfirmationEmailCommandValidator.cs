namespace StrikeDefender.Application.Accounts.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        public ResendConfirmationEmailCommandValidator()
        {
            RuleFor(x => x.Email)
             .NotEmpty()
             .EmailAddress();

        }

    }
}
