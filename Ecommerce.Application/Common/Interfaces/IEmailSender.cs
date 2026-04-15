namespace Ecommerce.Application.Common.Interfaces;
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default);
}
