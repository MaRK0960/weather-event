using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace weather_event
{
    public class WeatherUpdateSubscriptionServiceBus
    {
        [FunctionName("WeatherUpdateSubscriptionServiceBus")]
        public void Run([ServiceBusTrigger("%WeatherUpdateSubscriptionServiceBusName%", Connection = "WeatherUpdateSubscriptionServiceBus")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}