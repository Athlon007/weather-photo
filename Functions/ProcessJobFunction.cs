using System.Text;
using Athlon.WeatherPhoto.DTOs;
using Athlon.WeatherPhoto.Services;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Athlon.WeatherPhoto.Functions;

public class ProcessJobFunction(
    ILogger<ProcessImageFunction> logger,
    WeatherService weatherService,
    QueueServiceClient queueServiceClient)
{
    private const string StationName = "Schiphol";
    private const int StationId = 6240;

    [Function(nameof(ProcessJobFunction))]
    [QueueOutput("process-image-queue", Connection = "AzureWebJobsStorage")]
    public async Task Run([QueueTrigger("start-job-queue", Connection = "AzureWebJobsStorage")] string message)
    {
        try
        {
            logger.LogInformation("Processing message...");

            // Get the message data
            var request = JsonConvert.DeserializeObject<ProcessJobDto>(message);

            if (request == null)
            {
                logger.LogError("Request body was not provided.");
                return;
            }

            // Get the weather condition. For this example, we will use Schiphol weather data ( stationId: 6240)
            var station = await weatherService.GetWeatherStationData(StationId);
            string weatherDescription = station.weatherdescription.ToString();

            // Queue the image processing job
            var jobData = new
            {
                jobId = request.JobId,
                weatherDescription,
                stationName = StationName,
                station
            };
            var serialized = JsonConvert.SerializeObject(jobData);

            var queueClient = queueServiceClient.GetQueueClient("process-image-queue");
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized)));

            logger.LogInformation("Image processing job has been enqueued.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the image.");
        }
    }
}