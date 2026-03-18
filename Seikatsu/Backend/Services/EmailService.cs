using System.Net;
using System.Net.Mail;

namespace Seikatsu.Backend.Services
{
    public class EmailService(IConfiguration config): IEmailService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(config["Email:Host"])
            {
                Port = int.Parse(config["Email:Port"]!),
                Credentials = new NetworkCredential(
                    config["Email:Username"],
                    config["Email:Password"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(config["Email:From"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);
            await smtpClient.SendMailAsync(mail);
        }
    }
}
