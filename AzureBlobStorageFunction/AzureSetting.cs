using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageFunction
{
    public class AzureSetting
    {
        public static CloudBlobContainer GetCloudBlobContainer(ExecutionContext context)
        {
            var settings = Configuration.GetAzureSettings(context);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.AzureWebJobsStorage);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(settings.ContainerName);
            return container;
        }
    }
}
