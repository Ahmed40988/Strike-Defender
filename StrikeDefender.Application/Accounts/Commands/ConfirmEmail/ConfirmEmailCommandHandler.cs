using Microsoft.Extensions.Caching.Memory;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(
        UserManager<AppUser> userManager,
        IMemoryCache memoryCache
    ) : IRequestHandler<ConfirmEmailCommand, ErrorOr<Success>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;

        public async Task<ErrorOr<Success>> Handle(
            ConfirmEmailCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            if (user.EmailConfirmed)
                return Error.Conflict("Email.Confirmed", "Email already confirmed");

            var cachedOtp = _memoryCache.Get($"EmailOTP_{command.Email}")?.ToString();
            if (string.IsNullOrEmpty(cachedOtp))
                return Error.Validation("OTP.Expired", "OTP expired");

            if (cachedOtp != command.OTP)
                return Error.Validation("OTP.Invalid", "Invalid OTP");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, token);

            memoryCache.Remove($"EmailOTP_{command.Email}");
            return Result.Success;
        }
    }
}
