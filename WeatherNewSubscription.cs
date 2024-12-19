// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using weather_event.Service;

namespace weather_event
{
    public static class WeatherNewSubscription
    {
        [FunctionName("WeatherNewSubscription")]
        public static Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            string email = eventGridEvent.Data.ToObjectFromJson<string>();

            string emailBody = EmailTemplate.Get("weather-welcome-email-template.html");

            emailBody = emailBody.Replace("{SiteURL}", AppConfiguration.Get("Weather:Site"));

            string emailSubject = "Welcome to 7'tfa Weather Notification App!";

            try
            {
                return EmailSender.Send(email, emailSubject, emailBody);
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in SendEmail");
                return Task.FromException<Exception>(x);
            }
        }
    }
}