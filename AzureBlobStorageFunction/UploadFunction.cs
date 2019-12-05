using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorageFunction
{
    public static class UploadFunction
    {


        [FunctionName("UploadFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequest req,
             ILogger log, ExecutionContext context)
        {

            log.LogInformation("HTTP trigger function started processing a request.");
            try
            {
                var files = req.Form.Files;
                foreach (var file in files)
                {
                    await UploadFile(context, file, log).ConfigureAwait(false);
                }
                log.LogInformation("Http request processed has done.");
                return new OkObjectResult($"Files Count ({files.Count})  and names => [{string.Join(",", files.Select(s => s.FileName).ToList())}]");


            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
        private async static Task UploadFile(ExecutionContext context, IFormFile file, ILogger log)
        {
            log.LogInformation($"Uploading started for file:{file.FileName}");

            var container = AzureSetting.GetCloudBlobContainer(context);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

            using MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream).ConfigureAwait(false);
            await blockBlob.UploadFromStreamAsync(memoryStream).ConfigureAwait(false);
            log.LogInformation($"Uploading done for file:{file.FileName}");


        }

    }
}
