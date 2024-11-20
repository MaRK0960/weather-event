using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using weather_event.Models;

namespace weather_event
{
    public class WeatherTimer
    {
        [FunctionName("WeatherTimer")]
        public async Task Run([TimerTrigger("0 0 7,19 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                Weather weather = await GetWeather();

                if (weather == null)
                {
                    log.LogError("Failed to get weather forcast");
                    return;
                }

                Day day = weather.forecast.forecastday[0].day;

                TableServiceClient tableServiceClient = new("***REMOVED***");
                Response<TableItem> table = await tableServiceClient.CreateTableIfNotExistsAsync("Emails");

                TableClient tableClient = tableServiceClient.GetTableClient(table.Value.Name);

                List<string> emails = await tableClient.QueryAsync<Email>(maxPerPage: 1000)
                    .Select(e => e.EmailAddress)
                    .ToListAsync()
                    .ConfigureAwait(false);

                await SendEmail(emails,
                    "7'tfa Weather Notification",
                    $"Now {weather.current.temp_c:0.0}\u00B0C\n" +
                    $"Today {day.maxtemp_c:0.0}\u00B0C/{day.mintemp_c:0.0}\u00B0C\n" +
                    "May your 7'tfa stay eternally healthy!",
                    log);
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in ExecuteAsync");
            }
        }

        private async Task<Weather> GetWeather()
        {
            using HttpClient httpClient = new();

            HttpResponseMessage httpResponse = await httpClient.GetAsync("***REMOVED***");

            return await httpResponse.Content.ReadFromJsonAsync<Weather>();
        }

        private async Task SendEmail(List<string> toAddresses, string subject, string body, ILogger log)
        {
            try
            {
                MailMessage mail = new()
                {
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8,
                    From = new MailAddress("***REMOVED***"),
                    Subject = subject,
                    Body = body
                };

                foreach (string email in toAddresses)
                {
                    mail.Bcc.Add(email);
                }

                mail.Headers.Add("Content-Type", "text/html; charset=utf-8");

                SmtpClient smtpClient = new("smtp.azurecomm.net", 587)
                {
                    Credentials = new NetworkCredential("***REMOVED***", "***REMOVED***"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception x)
            {
                log.LogCritical(x, "Error in SendEmail");
            }
        }
    }
}
