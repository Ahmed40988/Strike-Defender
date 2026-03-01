using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(
        UserManager<AppUser> userManager
    ) : IRequestHandler<ResetPasswordCommand, ErrorOr<Success>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<ErrorOr<Success>> Handle(
            ResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            if (command.NewPassword != command.ConfirmPassword)
                return Error.Validation("Password.Mismatch", "Passwords do not match");

            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            var result = await _userManager.ResetPasswordAsync(
                user,
                command.Token,
                command.NewPassword);

            if (!result.Succeeded)
                return Error.Failure("Password.ResetFailed", "Reset password failed");

            return Result.Success;
        }
    }
}
