using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace weather_event
{
    public class WeatherTimer
    {
        [FunctionName("WeatherTimer")]
        public void Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            SendEmail("***REMOVED***", "Test Timer", "Test Timer").Wait();
        }

        private static async Task SendEmail(string toAddress, string subject, string body)
        {
            MailMessage mail = new()
            {
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress("***REMOVED***"),
                Subject = subject,
                Body = body
            };

            mail.To.Add(toAddress);
            mail.Headers.Add("Content-Type", "text/html; charset=utf-8");

            SmtpClient smtpClient = new("smtp.azurecomm.net", 587)
            {
                Credentials = new NetworkCredential("***REMOVED***", "***REMOVED***"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtpClient.SendMailAsync(mail);
        }
    }
}
