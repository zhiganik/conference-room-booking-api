using ConferenceRoomBooking.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")))
            .AddOpenApi();
    }
}