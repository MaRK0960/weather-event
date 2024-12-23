﻿using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace weather_event.Service
{
    internal static class EmailSender
    {
        public static async Task Send(string toAddress, string subject, string body)
        {
            using MailMessage mailMessage = CreateMessage(subject, body);

            mailMessage.To.Add(toAddress);

            await Send(mailMessage);
        }

        public static async Task Send(IEnumerable<string> toAddresses, string subject, string body)
        {
            using MailMessage mailMessage = CreateMessage(subject, body);

            foreach (string email in toAddresses)
            {
                mailMessage.Bcc.Add(email);
            }

            await Send(mailMessage);
        }

        private static MailMessage CreateMessage(string subject, string body)
        {
            return new MailMessage()
            {
                From = new MailAddress(AppConfiguration.Get("Weather:Communication:EmailAddress")),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
        }

        private static async Task Send(MailMessage mailMessage)
        {
            using SmtpClient smtpClient = new("smtp.azurecomm.net", 587)
            {
                Credentials = new NetworkCredential(AppConfiguration.Get("Weather:Communication:Username"), AppConfiguration.Get("Weather:Communication:Password")),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}