using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.CompleteProfile
{
    public class CompleteProfileCommandHandler(IFileHelperService fileHelperService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork) : IRequestHandler<CompleteProfileCommand, ErrorOr<Success>>
    {
        private readonly IFileHelperService _fileHelperService = fileHelperService;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<Success>> Handle(CompleteProfileCommand command, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            user.UpdateProfile(
                command.FullName,
                command.Phone,
                command.DateOfBirth
            );

            if (command.Image is not null)
            {
                user.PhotoURl = _fileHelperService.UploadFile(command.Image, "Users");
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Error.Failure("User.UpdateFailed", "Failed to update user profile");

            await _unitOfWork.CommitChangesAsync();

            return Result.Success;


        }
    }
}
