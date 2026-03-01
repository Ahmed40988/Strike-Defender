
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendResetPasswordEmail(AppUser user, string OTP);
        Task SendConfirmationEmail(AppUser user, string OTP);
    }
}
