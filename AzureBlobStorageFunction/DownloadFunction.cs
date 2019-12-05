using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureBlobStorageFunction
{
    public static class DownloadFunction
    {
        [FunctionName("DownloadFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            try
            {

                log.LogInformation("HTTP trigger function started processing a request.");

                string fileName = req.Query["fileName"];
                string downloadType = req.Query["downloadType"];

                if (string.IsNullOrWhiteSpace(fileName))
                    return new BadRequestObjectResult("file name can't be null or empty");

                if (!(downloadType == DownloadType.File.ToString() || downloadType == DownloadType.Base64.ToString()))
                {
                    return new BadRequestObjectResult("downloadType param value is incorrect. downloadType must be either File or Base64");
                }
                return await DownloadFile(fileName, downloadType, context).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
        public async static Task<IActionResult> DownloadFile(string fileName, string downloadType, ExecutionContext context)
        {
            var container = AzureSetting.GetCloudBlobContainer(context);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            if (await blockBlob.ExistsAsync().ConfigureAwait(false))
            {
                MemoryStream memoryStream = new MemoryStream();
                await blockBlob.DownloadToStreamAsync(memoryStream).ConfigureAwait(false);
                if (downloadType == DownloadType.File.ToString())
                {
                    return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = fileName
                    };
                }
                else if (downloadType == DownloadType.Base64.ToString())
                {
                    var fileContent = new { FileName = fileName, Base64 = memoryStream.ToArray() };
                    return new OkObjectResult(fileContent);
                }

            }
            return new BadRequestObjectResult("file not exist");
        }


    }
}
