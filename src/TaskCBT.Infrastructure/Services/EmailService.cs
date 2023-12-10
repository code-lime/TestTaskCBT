using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;

namespace TaskCBT.Infrastructure.Services;

public class EmailService(IOptions<EmailConfig> config) : IEmailService
{
    private readonly EmailConfig _config = config.Value;

    public async Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken)
    {
        using SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.Email, _config.Password)
        };
        MailAddress from = new MailAddress(_config.Email, _config.Name);
        MailAddress to = new MailAddress(email);
        using MailMessage message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = body
        };
        await smtp.SendMailAsync(message, cancellationToken);
    }
}
