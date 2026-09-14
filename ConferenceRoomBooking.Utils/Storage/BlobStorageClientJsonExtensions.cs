using System.Text.Json;

namespace ConferenceRoomBooking.Utils.Storage;

public static class BlobStorageClientJsonExtensions
{
    public static async Task<T?> DownloadJsonAsync<T>(
        this IBlobStorageClient client,
        string containerName,
        string blobName,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await client.DownloadAsync(containerName, blobName, cancellationToken);
        return JsonSerializer.Deserialize<T>(result.Content, options);
    }

    public static async Task UploadJsonAsync<T>(
        this IBlobStorageClient client,
        string containerName,
        string blobName,
        T value,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
        stream.Position = 0;

        await client.UploadAsync(containerName, blobName, stream, "application/json", cancellationToken);
    }
}
