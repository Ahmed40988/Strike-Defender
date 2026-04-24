using StrikeDefender.Application.Accounts.AccountDTO;

namespace StrikeDefender.Application.Accounts.Commands.GetProfile

{
    public record GetProfileCommand(string UserId) : IRequest<ErrorOr<UserProfileDto>>;
}
