using ConferenceRoomBooking.Api.Middleware;
using ConferenceRoomBooking.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        
        return services
            .AddExceptionHandler()
            .AddDb(config)
            .AddOpenApi();
    }

    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    public static IServiceCollection AddDb(this IServiceCollection services, IConfiguration config)
    {
        return services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    }
}