using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using weather_event.Service;

namespace weather_event
{
    public static class WeatherEvent
    {
        [FunctionName("WeatherEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string email = req.Query["email"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            email ??= data?.name;

            string emailBody = EmailTemplate.Get("weather-welcome-email-template.html");

            emailBody = emailBody.Replace("{SiteURL}", AppConfiguration.Get("Weather:Site"));

            try
            {
                await EmailSender.Send(email, "Welcome to 7'tfa Weather Notification App!", emailBody);
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in SendEmail");
                return new ExceptionResult(x, true);
            }

            return new OkResult();
        }
    }
}
