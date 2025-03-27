using System.Net;
using System.Net.Mail;

namespace Company.G03.PL.Helpers
    {
    public class EmailSettings
        {

        public static bool SendEmail(Email email)
            {
            try
                {
                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("ahmedh.emam99@gmail.com", "xqyaiqohmvjohwhd");
                client.Send("ahmedh.emam99@gmail.com", email.EmailAddress, email.Subject, email.Body);
                return true;
                }

            catch (Exception e)
                {
                return false;
                }

            }
        }
    }
