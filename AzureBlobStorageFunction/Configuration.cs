using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
namespace AzureBlobStorageFunction
{
    public enum DownloadType
    {
        File,
        Base64
    }
    public class Configuration
    {
        public static (string AzureWebJobsStorage, string ContainerName) GetAzureSettings(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                        .SetBasePath(context.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

            return (config["AzureWebJobsStorage"], config["ContainerName"]);
        }
    }

}
