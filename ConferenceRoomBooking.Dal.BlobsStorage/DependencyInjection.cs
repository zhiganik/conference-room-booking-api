using ConferenceRoomBooking.Dal.BlobsStorage.Configurations;
using ConferenceRoomBooking.Utils.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceRoomBooking.Dal.BlobsStorage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalBlobsStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageSettings>(configuration.GetSection(BlobStorageSettings.SectionName));
        services.AddScoped<IBlobStorageClient, AzureBlobStorageClient>();

        return services;
    }
}
