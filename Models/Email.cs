using Azure;
using Azure.Data.Tables;
using System;

namespace weather_event.Models
{
    public class Email : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string EmailAddress { get; set; }
    }
}
