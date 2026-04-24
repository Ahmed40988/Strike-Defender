using StrikeDefender.Application.Accounts.AccountDTO;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.GetProfile
{
    public class GetProfileCommandHandler( UserManager<AppUser> userManager)
        : IRequestHandler<GetProfileCommand, ErrorOr<UserProfileDto>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;


        public async  Task<ErrorOr<UserProfileDto>> Handle(GetProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Error.NotFound("User.NotFound", "User with the specified ID was not found.");

            return new UserProfileDto(
        user.Id,
        user.Email!,
        user.FullName,
        $"https://strike-defender-v1.runasp.net/api/Files/image/{user.PhotoURl}",
        user.DateOfBirth,
        user.CreatedOn
    );
        }
    }
}

