using Microsoft.Extensions.Caching.Memory;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.VerifyForgotPasswordOtp
{
    public class VerifyForgotPasswordOtpCommandHandler(
        UserManager<AppUser> userManager,
        IMemoryCache memoryCache
    ) : IRequestHandler<VerifyForgotPasswordOtpCommand, ErrorOr<string>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;

        public async Task<ErrorOr<string>> Handle(
            VerifyForgotPasswordOtpCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            var cachedOtp = _memoryCache.Get($"ForgetPassword{command.Email}")?.ToString();

            if (string.IsNullOrEmpty(cachedOtp))
                return Error.Validation("OTP.Expired", "OTP expired");

            if (cachedOtp != command.OTP)
                return Error.Validation("OTP.Invalid", "Invalid OTP");

            _memoryCache.Remove($"ForgetPassword{command.Email}");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
    }
}
