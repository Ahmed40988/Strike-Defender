namespace StrikeDefender.Application.Accounts.Commands.CompleteProfile
{
    public class CompleteProfileCommandValidator : AbstractValidator<CompleteProfileCommand>
    {
        public CompleteProfileCommandValidator()
        {

            RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");


            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(\+?\d{10,15})$").WithMessage("Phone number must be valid and contain 10 to 15 digits.");


            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(BeAValidAge).WithMessage("You must be at least 13 years old.");


            RuleFor(x => x.Image)
                .NotNull().WithMessage("Profile image is required.")
                .Must(file => file.Length > 0).WithMessage("Profile image cannot be empty.")
                .Must(file => file.Length <= 2 * 1024 * 1024).WithMessage("Profile image must be less than 2 MB.")
                .Must(file => new[] { ".jpg", ".jpeg", ".png" }
                    .Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Only .jpg, .jpeg, and .png formats are allowed.");
        }

        private bool BeAValidAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age >= 13;

        }


    }
}
