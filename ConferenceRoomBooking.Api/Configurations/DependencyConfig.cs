using ConferenceRoomBooking.Api.Middleware;
using ConferenceRoomBooking.Application.Validators.Rooms;
using ConferenceRoomBooking.DataLayer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ConferenceRoomBooking.Api.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        
        return services
            .AddExceptionHandler()
            .AddDb(config)
            .AddValidation()
            .AddOpenApi();
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    private static IServiceCollection AddDb(this IServiceCollection services, IConfiguration config)
    {
        return services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    }
    
    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssembly(typeof(Validators).Assembly);
    }
}