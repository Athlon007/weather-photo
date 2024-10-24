using Athlon.WeatherPhoto.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        var azureStorageConfig = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        if (string.IsNullOrEmpty(azureStorageConfig))
        {
            throw new InvalidOperationException("AzureWebJobsStorage is not set.");
        }

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton(new BlobServiceClient(azureStorageConfig));
        services.AddSingleton<BlobStorageService>();
        services.AddSingleton<WeatherService>();
        services.AddSingleton(_ => new ImageService(new HttpClient()));
        services.AddSingleton(_ => new QueueServiceClient(azureStorageConfig));
    })
    .Build();

host.Run();