namespace ConferenceRoomBooking.Api.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddOpenApi();
    }
}