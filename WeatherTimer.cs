using Azure.Data.AppConfiguration;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using weather_event.Models;
using weather_event.Service;

namespace weather_event
{
    public class WeatherTimer
    {
        [FunctionName("WeatherTimer")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
                return;

            try
            {
                Weather weather = await GetWeather();

                if (weather == null)
                {
                    log.LogError("Failed to get weather forcast");
                    return;
                }

                ConfigurationClient configurationClient = new(new Uri(AppConfiguration.Get("Weather:Configuration:URI")), new DefaultAzureCredential());

                Container container = await GetContainer(configurationClient);

                int currentHour = DateTime.UtcNow.Hour;

                FeedIterator<string> feed = container
                    .GetItemLinqQueryable<WeatherUser>()
                    .Where(s => s.NotificationTime.Any(t => t == currentHour))
                    .Select(s => s.Email)
                    .ToFeedIterator();

                string emailBody = ConstructEmailBody(weather);

                while (feed.HasMoreResults)
                {
                    FeedResponse<string> results = await feed.ReadNextAsync();

                    if (results.Count > 0)
                        await EmailSender.Send(results, "7'tfa Weather Notification", emailBody);
                }
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in ExecuteAsync");
            }
        }

        private static async Task<Weather> GetWeather()
        {
            using HttpClient httpClient = new();

            HttpResponseMessage httpResponse = await httpClient.GetAsync(AppConfiguration.Get("Weather:External:API"));

            return await httpResponse.Content.ReadFromJsonAsync<Weather>();
        }

        private static async Task<Container> GetContainer(ConfigurationClient configurationClient)
        {
            Azure.Response<ConfigurationSetting> cosmosUriSetting = await configurationClient.GetConfigurationSettingAsync("Weather-Cosmos-URI");
            string cosmosUri = cosmosUriSetting.Value.Value;

            CosmosClient cosmosClient = new(
                accountEndpoint: cosmosUri,
                tokenCredential: new DefaultAzureCredential()
            );

            Database database = cosmosClient.GetDatabase("Weather");
            Container container = database.GetContainer("Users");

            return container;
        }

        private static string ConstructEmailBody(Weather weather)
        {
            Day day = weather.forecast.forecastday[0].day;

            string emailBody = EmailTemplate.Get("weather-notification-email-template.html");

            emailBody = emailBody.Replace("{SiteURL}", AppConfiguration.Get("Weather:Site"));
            emailBody = emailBody.Replace("{CurrentTemp}", $"{weather.current.temp_c:0.0}");
            emailBody = emailBody.Replace("{MinTemp}", $"{day.mintemp_c:0.0}");
            emailBody = emailBody.Replace("{MaxTemp}", $"{day.maxtemp_c:0.0}");

            return emailBody;
        }
    }
}