using Azure.Messaging.EventGrid;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using weather_event.Service;

namespace weather_event
{
    public static class WeatherUpdateSubscription
    {
        [FunctionName("WeatherUpdateSubscription")]
        public static Task Run([EventHubTrigger("%WeatherUpdateSubscriptionEventHubName%", Connection = "WeatherUpdateSubscriptionEventHub")] EventData[] events, ILogger log)
        {
            List<string> emails = events
                .Select(e => e.EventBody.ToObjectFromJson<EventGridEvent[]>())
                .SelectMany(es => es.Select(e => e.Data.ToObjectFromJson<string>()))
                .ToList();

            log.LogInformation(string.Join(',', emails));

            string emailBody = EmailTemplate.Get("weather-modified-email-template.html");

            emailBody = emailBody.Replace("{SiteURL}", AppConfiguration.Get("Weather:Site"));

            string emailSubject = "7'tfa Weather Notification App Subscription Updated!";

            try
            {
                return EmailSender.Send(emails, emailSubject, emailBody);
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in SendEmail");
                return Task.FromException<Exception>(x);
            }
        }
    }
}