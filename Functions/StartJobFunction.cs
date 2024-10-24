using System.Text;
using Athlon.WeatherPhoto.DTOs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Athlon.WeatherPhoto.Functions;

public class StartJobFunction(ILogger<StartJobFunction> logger, QueueServiceClient queueServiceClient)
{

    [Function("StartJob")]
    [QueueOutput("start-job-queue")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        try
        {
            logger.LogInformation("Starting a new job...");

            // Initialize the job.
            var jobId = Guid.NewGuid().ToString();

            // Serialize the request object with the jobId.
            var jobData = new ProcessJobDto(jobId);

            var message = JsonConvert.SerializeObject(jobData);

            var queueClient = queueServiceClient.GetQueueClient("start-job-queue");
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));

            logger.LogInformation("Job request has been enqueued.");

            // Return the job ID.
            var response = new
            {
                jobId
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while starting a new job.");
            return new StatusCodeResult(500);
        }
    }
}