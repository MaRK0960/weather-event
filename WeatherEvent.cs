using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace weather_event
{
    public static class WeatherEvent
    {
        [FunctionName("WeatherEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string email = req.Query["email"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            email = email ?? data?.name;

            try
            {
                await SendEmail(email,
                    "Welcome to 7'tfa Weather Notification App!",
                    "You just registered to our magnificent 7'tfa Weather Notification App!\nEnjoy weather notifications every hour!");
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in SendEmail");
                return new ExceptionResult(x, true);
            }

            return new OkResult();
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
