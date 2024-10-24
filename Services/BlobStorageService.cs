using Athlon.WeatherPhoto.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Athlon.WeatherPhoto.Services;

public class BlobStorageService(BlobServiceClient blobServiceClient) : IService
{
    public async Task<BlobContainerClient> GetBlobContainerClient(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }

    public Uri GenerateLinkWithSasToken(BlobContainerClient containerClient, string blobName, BlobSasBuilder? builder = null)
    {
        var blobClient = containerClient.GetBlobClient(blobName);

        if (builder == null)
        {
            builder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        }

        return blobClient.GenerateSasUri(builder);
    }

    public bool BlobExists(BlobContainerClient containerClient, string blobName)
    {
        foreach (var blobItem in containerClient.GetBlobs())
        {
            if (blobItem.Name.Contains(blobName))
            {
                return true;
            }
        }

        return false;
    }
}