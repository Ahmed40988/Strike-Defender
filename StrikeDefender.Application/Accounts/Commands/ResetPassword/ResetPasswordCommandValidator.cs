namespace StrikeDefender.Application.Accounts.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token).NotEmpty();

            RuleFor(x => x.Email)
  .NotEmpty()
  .EmailAddress();


            RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(12).WithMessage("Password must not exceed 12 characters")
            .Matches(PasswordRegexPatterns.Password).WithMessage("Password must contain uppercase and lowercase letters, numbers, and special characters")
            .Matches(x => x.ConfirmPassword).WithMessage("NewPassword and ConfirmPassword  must be Equal");


        }
    }
    public static class PasswordRegexPatterns
    {
        public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
    }
}
