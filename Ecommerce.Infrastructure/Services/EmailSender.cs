using Ecommerce.Application.Common.Options;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Ecommerce.Infrastructure.Services;
public class EmailSender(IOptions<EmailOptions> emailOptions) : IEmailSender
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_emailOptions.Mail),
            Subject = subject,
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = body
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.CheckCertificateRevocation = false;
        smtp.Connect(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_emailOptions.Mail, _emailOptions.Password);

        await smtp.SendAsync(message);

        smtp.Disconnect(true);
    }
}
