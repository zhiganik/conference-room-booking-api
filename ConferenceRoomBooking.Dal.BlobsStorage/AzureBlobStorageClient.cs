using System.Runtime.CompilerServices;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using ConferenceRoomBooking.Dal.BlobsStorage.Configurations;
using ConferenceRoomBooking.Utils.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlobDownloadResult = ConferenceRoomBooking.Utils.Storage.BlobDownloadResult;

namespace ConferenceRoomBooking.Dal.BlobsStorage;

public class AzureBlobStorageClient(IOptions<BlobStorageSettings> options,
    ILogger<AzureBlobStorageClient> logger) : IBlobStorageClient
{
    private readonly Lazy<BlobServiceClient> _serviceClient = new(() =>
    {
        var accountUrl = options.Value.AccountUrl;

        if (string.IsNullOrWhiteSpace(accountUrl))
        {
            throw new InvalidOperationException(
                $"{BlobStorageSettings.SectionName}:{nameof(BlobStorageSettings.AccountUrl)} is not configured.");
        }

        return new BlobServiceClient(new Uri(accountUrl), new DefaultAzureCredential());
    });

    private BlobServiceClient ServiceClient => _serviceClient.Value;

    public async Task EnsureContainerExistsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        var container = ServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Container {ContainerName} is ready", containerName);
    }

    public async Task UploadAsync(string containerName, string blobName, Stream content, string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        var blob = ServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        var uploadOptions = new BlobUploadOptions();
        if (contentType is not null)
        {
            uploadOptions.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blob.UploadAsync(content, uploadOptions, cancellationToken);
        logger.LogInformation("Blob {ContainerName}/{BlobName} uploaded", containerName, blobName);
    }

    public async Task<BlobDownloadResult> DownloadAsync(string containerName, string blobName,
        CancellationToken cancellationToken = default)
    {
        var blob = ServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);
        Azure.Storage.Blobs.Models.BlobDownloadResult result = await blob.DownloadContentAsync(cancellationToken);

        return new BlobDownloadResult(result.Content.ToArray(), result.Details.ContentType);
    }

    public async Task<bool> ExistsAsync(string containerName, string blobName,
        CancellationToken cancellationToken = default)
    {
        var blob = ServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);
        return await blob.ExistsAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string containerName, string blobName,
        CancellationToken cancellationToken = default)
    {
        var blob = ServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);
        var deleted = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);

        logger.LogInformation("Blob {ContainerName}/{BlobName} deleted: {Deleted}", containerName, blobName, deleted);
        return deleted;
    }

    public async IAsyncEnumerable<BlobItemInfo> ListAsync(string containerName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var container = ServiceClient.GetBlobContainerClient(containerName);

        await foreach (var blob in container.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            yield return new BlobItemInfo(blob.Name, blob.Properties.ContentLength, blob.Properties.LastModified);
        }
    }

    public async Task<Uri> GetTemporaryReadUrlAsync(string containerName, string blobName, TimeSpan validFor,
        CancellationToken cancellationToken = default)
    {
        var blob = ServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        var delegationKey = await ServiceClient.GetUserDelegationKeyAsync(
            startsOn: DateTimeOffset.UtcNow.AddMinutes(-5),
            expiresOn: DateTimeOffset.UtcNow.Add(validFor),
            cancellationToken);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return new BlobUriBuilder(blob.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(delegationKey, ServiceClient.AccountName)
        }.ToUri();
    }
}
