using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using weather_event.Service;

namespace weather_event
{
    public class WeatherUpdateSubscriptionQueue
    {
        [FunctionName("WeatherUpdateSubscriptionQueue")]
        public Task Run([QueueTrigger("weather-modified-subscription", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            EventGridEvent eventGridEvent = JsonSerializer.Deserialize<EventGridEvent>(myQueueItem);

            string email = eventGridEvent.Data.ToObjectFromJson<string>();

            string emailBody = EmailTemplate.Get("weather-modified-email-template.html");

            emailBody = emailBody.Replace("{SiteURL}", AppConfiguration.Get("Weather:Site"));

            string emailSubject = "7'tfa Weather Notification App Subscription Updated!";

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