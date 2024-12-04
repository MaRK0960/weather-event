using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace weather_event.Service
{
    internal static class EmailTemplate
    {
        public static string Get(string name)
        {
            BlobServiceClient blobServiceClient = new(AppConfiguration.Get("CUSTOMCONNSTR_Weather:Table:ConnectionString"));
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("emails");
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            BlobDownloadResult downloaded = blobClient.DownloadContent().Value;
            return downloaded.Content.ToString();
        }
    }
}