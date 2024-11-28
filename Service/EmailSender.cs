using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace weather_event.Service
{
    internal static class EmailSender
    {
        public static Task Send(string toAddress, string subject, string body)
        {
            using MailMessage mailMessage = CreateMessage(subject, body);

            mailMessage.To.Add(toAddress);

            return Send(mailMessage);
        }

        public static Task Send(List<string> toAddresses, string subject, string body)
        {
            using MailMessage mailMessage = CreateMessage(subject, body);

            foreach (string email in toAddresses)
            {
                mailMessage.Bcc.Add(email);
            }

            return Send(mailMessage);
        }

        private static MailMessage CreateMessage(string subject, string body)
        {
            return new MailMessage()
            {
                From = new MailAddress("***REMOVED***"),
                Subject = subject,
                Body = body
            };
        }

        private static Task Send(MailMessage mailMessage)
        {
            using SmtpClient smtpClient = new("smtp.azurecomm.net", 587)
            {
                Credentials = new NetworkCredential("***REMOVED***", "***REMOVED***"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
