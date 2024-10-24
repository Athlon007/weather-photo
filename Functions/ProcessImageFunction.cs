using Athlon.WeatherPhoto.DTOs;
using Athlon.WeatherPhoto.Helpers;
using Athlon.WeatherPhoto.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Athlon.WeatherPhoto.Functions;

public class ProcessImageFunction(
    ILogger<ProcessImageFunction> logger,
    BlobStorageService blobStorageService,
    ImageService imageService)
{

    [Function(nameof(ProcessImageFunction))]
    public async Task Run([QueueTrigger("process-image-queue", Connection = "AzureWebJobsStorage")] string message)
    {
        try
        {
            logger.LogInformation("Processing message...");

            // Get the message data
            var request = JsonConvert.DeserializeObject<ProcessImageDto>(message);

            if (request == null)
            {
                logger.LogError("Request body was not provided.");
                return;
            }

            // Get the blob container
            var containerClient =
                await blobStorageService.GetBlobContainerClient(
                    Environment.GetEnvironmentVariable("WeatherPhotosContainer") ?? "weather-photos");

            var weatherDescription = request.WeatherDescription;
            // Get image based on the weather description
            var image = await imageService.GetRandomImage(weatherDescription);
            // Downscale the image to 800x600, so it's not too large.
            var imageDownscaled = ImageHelper.DownscaleImage(image, 800, 600);

            var station = request.Station;

            // Add text to the image
            const string white = "#FFFFFF";
            var imageWithText = ImageHelper.AddTextToImage(
                imageDownscaled,
                (request.StationName, (10, 10), 40, white),
                (request.WeatherDescription, (10, 50), 40, white),
                (station.temperature + "°C", (10, 90), 40, white),
                (station.windspeed + " km/h", (10, 130), 40, white),
                (station.winddirection, (10, 170), 40, white),
                (station.airpressure + " hPa", (10, 210), 40, white),
                (station.humidity + "%", (10, 250), 40, white)
            );

            // Upload the image to the container
            var blobClient = containerClient.GetBlobClient($"{request.JobId}.jpg");
            await blobClient.UploadAsync(imageWithText, true);

            logger.LogInformation("Processsing completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the image.");
        }
    }
}