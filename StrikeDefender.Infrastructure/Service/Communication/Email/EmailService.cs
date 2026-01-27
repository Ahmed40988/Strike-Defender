using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefenders.Infrastructure.Service.Communication.Email;

namespace StrikeDefender.Infrastructure.Service.Communication.Email;

public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailSender,IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        _logger.LogInformation("Sending email to {email}", email);

        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }

    public async Task SendResetPasswordEmail(string Email,string FullName, string OTP)
    {

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", templateModel: new Dictionary<string, string>
                {
                    { "{{name}}",FullName },
                    { "{{OTP}}", $"{OTP}" }
                }
        );

        await SendEmailAsync(Email!, "✅StrikeDefender: Change Password", emailBody);

        await Task.CompletedTask;
    }

    public async Task SendConfirmationEmail(string Email, string FullName, string OTP)
    {

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", templateModel: new Dictionary<string, string>
                {
                { "{{name}}",FullName },
                { "{{OTP}}", $"{OTP}" }
                }
        );
        await SendEmailAsync(Email!, "✅  StrikeDefender: Email Confirmation", emailBody);

        await Task.CompletedTask;
    }
}