using System;

namespace weather_event.Service
{
    internal static class AppConfiguration
    {
        public static string Get(string key) =>
            Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
    }
}