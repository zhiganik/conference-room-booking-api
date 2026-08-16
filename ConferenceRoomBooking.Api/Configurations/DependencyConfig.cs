using ConferenceRoomBooking.Api.Middleware;
using ConferenceRoomBooking.Application.BusinessLogic.Availability;
using ConferenceRoomBooking.Application.BusinessLogic.Pricing;
using ConferenceRoomBooking.Application.Orchestrators.Booking;
using ConferenceRoomBooking.Application.Orchestrators.Rooms;
using ConferenceRoomBooking.Application.Orchestrators.ServiceOptions;
using ConferenceRoomBooking.DataLayer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Validators = ConferenceRoomBooking.Application.Validators.Validators;

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
            .AddSwaggerDocs()
            .AddBusinessLogic()
            .AddOrchestrators();
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
        services.AddValidatorsFromAssembly(typeof(Validators).Assembly);
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddOrchestrators(this IServiceCollection services)
    {
        services.AddScoped<IRoomOrchestrator, RoomOrchestrator>();
        services.AddScoped<IServiceOptionOrchestrator, ServiceOptionOrchestrator>();
        services.AddScoped<IBookingOrchestrator, BookingOrchestrator>();
        return services;
    }
    
    private static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<IRentalPriceCalculator, RentalPriceCalculator>();
        services.AddSingleton<IRoomAvailabilityChecker, RoomAvailabilityChecker>();
        return services;
    }
    
    private static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Room Booking API",
                Version = "v1",
                Description = "API for managing conference rooms, bookings, and rental pricing."
            });
        });

        return services;
    }
}