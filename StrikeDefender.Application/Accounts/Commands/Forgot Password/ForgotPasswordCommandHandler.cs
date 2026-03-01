using Microsoft.Extensions.Caching.Memory;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler(
       UserManager<AppUser> userManager,
       IMemoryCache memoryCache,
       IEmailService emailService
   ) : IRequestHandler<ForgotPasswordCommand, ErrorOr<string>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IEmailService _emailService = emailService;

        public async Task<ErrorOr<string>> Handle(
            ForgotPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);

            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            var OTP = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"ForgetPassword{command.Email}", OTP, TimeSpan.FromMinutes(60));

            await _emailService.SendResetPasswordEmail(user, OTP);

            return "Verification code sent to your email";
        }
    }
}
