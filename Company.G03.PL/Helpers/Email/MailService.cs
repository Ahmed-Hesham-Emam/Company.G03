using Company.G03.PL.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Company.G03.PL.Helpers.Email
    {
    public class MailService(IOptions<EmailSettings> _options) : IEmailService
        {


        public void SendEmail(Email email)
            {

            // Construct the email message
            var mail = new MimeMessage();
            mail.Subject = email.Subject;
            mail.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));
            mail.To.Add(MailboxAddress.Parse(email.EmailAddress));
            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();

            // Establish the SMTP client
            using var smtp = new SmtpClient();
            smtp.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Value.Email, _options.Value.Password);

            // Send the email
            smtp.Send(mail);

            }

        }
    }
