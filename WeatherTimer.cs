using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public async Task Run([TimerTrigger("0 0 7,19 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                myTimer.
                Weather weather = await GetWeather();

                if (weather == null)
                {
                    log.LogError("Failed to get weather forcast");
                    return;
                }

                Day day = weather.forecast.forecastday[0].day;

                TableServiceClient tableServiceClient = new(AppConfiguration.Get("CUSTOMCONNSTR_Weather:Table:ConnectionString"));
                Response<TableItem> table = await tableServiceClient.CreateTableIfNotExistsAsync("Emails");

                TableClient tableClient = tableServiceClient.GetTableClient(table.Value.Name);

                List<string> emails = await tableClient.QueryAsync<Email>(maxPerPage: 1000)
                    .Select(e => e.EmailAddress)
                    .ToListAsync()
                    .ConfigureAwait(false);

                await EmailSender.Send(emails,
                    "7'tfa Weather Notification",
                    $"Now {weather.current.temp_c:0.0}\u00B0C\n" +
                    $"Today {day.maxtemp_c:0.0}\u00B0C/{day.mintemp_c:0.0}\u00B0C\n" +
                    $"Invite new friends to our <a href=\"{AppConfiguration.Get("Weather:Site")}\">7'tfa Weather Notification App!</a>" +
                    "May your 7'tfa stay eternally healthy!");
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in ExecuteAsync");
            }
        }

        private async Task<Weather> GetWeather()
        {
            using HttpClient httpClient = new();

            HttpResponseMessage httpResponse = await httpClient.GetAsync(AppConfiguration.Get("Weather:External:API"));

            return await httpResponse.Content.ReadFromJsonAsync<Weather>();
        }
    }
}
