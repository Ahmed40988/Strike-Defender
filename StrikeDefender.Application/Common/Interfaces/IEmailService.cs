
namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendResetPasswordEmail(string Email, string FullName, string OTP);
        Task SendConfirmationEmail(string Email, string FullName, string OTP);
    }
}
