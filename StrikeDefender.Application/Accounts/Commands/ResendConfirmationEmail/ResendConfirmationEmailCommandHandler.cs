using Microsoft.Extensions.Caching.Memory;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Accounts.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandHandler(
  UserManager<AppUser> userManager,
  IMemoryCache memoryCache,
  IEmailService emailService
) : IRequestHandler<ResendConfirmationEmailCommand, ErrorOr<Success>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IEmailService _emailService = emailService;

        public async Task<ErrorOr<Success>> Handle(
            ResendConfirmationEmailCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);

            if (user is null)
            {
                return Result.Success;
            }

            if (user.EmailConfirmed)
            {
                return Error.Conflict(
                    code: "User.EmailAlreadyConfirmed",
                    description: "Email is already confirmed."
                );
            }

            var OTP = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set(
                $"EmailOTP_{command.Email}",
                OTP,
                TimeSpan.FromMinutes(5)
            );

            await _emailService.SendConfirmationEmail(user, OTP);

            return Result.Success;
        }
    }

}
