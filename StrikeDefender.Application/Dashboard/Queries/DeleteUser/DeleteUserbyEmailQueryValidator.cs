namespace StrikeDefender.Application.Dashboard.Queries.DeleteUser
{
    public class DeleteUserbyEmailQueryValidator : AbstractValidator<DeleteUserbyEmailQuery>
    {
        public DeleteUserbyEmailQueryValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty()
                .NotNull();




        }

    }
}
