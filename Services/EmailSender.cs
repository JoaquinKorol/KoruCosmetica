using KoruCosmetica.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace KoruCosmetica.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _mail;
        private readonly string _contraseña;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _mail = smtpSettings.Value.Mail;
            _contraseña = smtpSettings.Value.Contraseña;
            
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = _mail;
            var pw = _contraseña;

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message));

        }
    }
}
