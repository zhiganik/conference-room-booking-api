namespace ConferenceRoomBooking.Utils.Storage;

/// <summary>
/// Provider-agnostic abstraction over blob storage (Azure Blob Storage, S3, Cloudflare R2, …).
/// Callers address blobs purely by container/blob name — no provider-specific types leak through
/// this interface, so swapping the implementation project is enough to switch providers.
/// </summary>
public interface IBlobStorageClient
{
    /// <summary>
    /// Creates the container if it doesn't already exist. Idempotent.
    /// </summary>
    Task EnsureContainerExistsAsync(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads <paramref name="content"/> as <paramref name="blobName"/>, overwriting any existing blob
    /// with the same name.
    /// </summary>
    Task UploadAsync(
        string containerName,
        string blobName,
        Stream content,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob's full content into memory. Not suitable for very large blobs.
    /// </summary>
    Task<BlobDownloadResult> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a blob exists.
    /// </summary>
    Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob. Returns false if it didn't exist.
    /// </summary>
    Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the blobs in a container.
    /// </summary>
    IAsyncEnumerable<BlobItemInfo> ListAsync(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a time-limited, read-only URL for a single blob (e.g. an Azure User Delegation SAS, or an
    /// S3/R2 presigned URL), so a client can download it directly without proxying through this service.
    /// </summary>
    Task<Uri> GetTemporaryReadUrlAsync(
        string containerName,
        string blobName,
        TimeSpan validFor,
        CancellationToken cancellationToken = default);
}
