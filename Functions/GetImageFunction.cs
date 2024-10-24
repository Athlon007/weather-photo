using Athlon.WeatherPhoto.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Athlon.WeatherPhoto.Functions;

public class GetImageFunction(ILogger<GetImageFunction> logger, BlobStorageService blobStorageService)
{
    [Function("GetImage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        try
        {
            logger.LogInformation("Getting image...");

            // Get job ID from query string
            string? jobId = req.Query["jobId"];

            if (string.IsNullOrEmpty(jobId))
            {
                return new BadRequestObjectResult("Please pass a job ID on the query string");
            }

            // Check if the image is ready
            // Check blob storage for job ID
            var containerClient =
                await blobStorageService.GetBlobContainerClient(
                    Environment.GetEnvironmentVariable("WeatherPhotosContainer") ?? "weather-photos");
            if (!blobStorageService.BlobExists(containerClient, jobId))
            {
                return new NotFoundObjectResult("Job or image not found.");
            }

            // Generate a URL for the image
            var url = blobStorageService.GenerateLinkWithSasToken(containerClient, $"{jobId}.jpg");

            return new OkObjectResult(new { url });
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the image.");
            return new StatusCodeResult(500);
        }
    }
}