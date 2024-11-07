using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace weather_event
{
    public class WeatherTimer
    {
        [FunctionName("WeatherTimer")]
        public void Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            ArmClient client = new(new DefaultAzureCredential());

            ContainerGroupResource containerGroup = client.GetContainerGroupResource(
                ResourceIdentifier.Parse("***REMOVED***"));

            containerGroup.Start(WaitUntil.Started);
        }
    }
}