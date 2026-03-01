using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using StrikeDefender.Domain.Users;
using StrikeDefender.Application.Accounts.Commands.CreateAccount;

namespace StrikeDefender.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler(IUserRepository userRepository
        , IUnitOfWork unitOfWork
        , UserManager<AppUser> userManager
        , ITokenService tokenService
        , IMemoryCache memoryCache
        , IEmailService emailService) : IRequestHandler<CreateAccountCommand, ErrorOr<Success>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IEmailService _emailService = emailService;

        public async Task<ErrorOr<Success>> Handle(CreateAccountCommand Command, CancellationToken cancellationToken)
        {
            var IsuserDeleted = await _userRepository.ExistSameEmailandDeletedAsync(Command.Email, cancellationToken);
            //Restore user if register by email deleted by admin
            if (IsuserDeleted)
            {

                var Deleteduser = await _userManager.FindByEmailAsync(Command.Email);
                Deleteduser.Restore("System");

                var resetToken =
         await _userManager.GeneratePasswordResetTokenAsync(Deleteduser);

                var resetResult = await _userManager.ResetPasswordAsync(
                    Deleteduser,
                    resetToken,
                    Command.Passsword);

                if (!resetResult.Succeeded)
                {
                    return resetResult.Errors
                        .Select(e => Error.Validation(
                            code: $"Identity.{e.Code}",
                            description: e.Description))
                        .First();
                }
                var otp = new Random().Next(100000, 999999).ToString();
                _memoryCache.Set($"EmailOTP_{Command.Email}", otp, TimeSpan.FromMinutes(5));
                await _emailService.SendConfirmationEmail(Deleteduser, otp);
                return Result.Success;

            }



            var userExists = await _userRepository.ExistsByEmailAsync(Command.Email, cancellationToken);

            if (userExists)
            {
                return Error.Conflict(
                    code: "User.Dublicated",
                    description: "Email is already exist");
            }

            var user = new AppUser(Command.Email);
            var result = await _userManager.CreateAsync(user, Command.Passsword);

            if (result.Succeeded)
            {
                var otp = new Random().Next(100000, 999999).ToString();
                _memoryCache.Set($"EmailOTP_{Command.Email}", otp, TimeSpan.FromMinutes(5));
                await _emailService.SendConfirmationEmail(user, otp);
                return Result.Success;
            }

            return result.Errors
         .Select(e => Error.Validation(
             code: $"Identity.{e.Code}",
             description: e.Description
         )).FirstOrDefault();

        }
    }
}
