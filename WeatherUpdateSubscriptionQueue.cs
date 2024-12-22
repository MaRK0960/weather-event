using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace weather_event
{
    public class WeatherUpdateSubscriptionQueue
    {
        [FunctionName("WeatherUpdateSubscriptionQueue")]
        public void Run([QueueTrigger("weather-modified-subscription", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}