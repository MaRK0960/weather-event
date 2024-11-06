using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace weather_event
{
    public class WeatherTimer
    {
        [FunctionName("WeatherTimer")]
        public void Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
