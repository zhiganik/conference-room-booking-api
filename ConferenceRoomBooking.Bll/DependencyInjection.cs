using ConferenceRoomBooking.Bll.Analytics;
using ConferenceRoomBooking.Bll.Auth;
using ConferenceRoomBooking.Bll.Bookings;
using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Rooms;
using ConferenceRoomBooking.Bll.ServiceOptions;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceRoomBooking.Bll;

/// <summary>Registers the business logic layer: domain services and every manager.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<IRentalPriceCalculator, RentalPriceCalculator>();
        services.AddSingleton<IRoomBookingLock, RoomBookingLock>();
        services.AddTransient<IJwtIssuer, JwtIssuer>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IRoomManager, RoomManager>();
        services.AddScoped<IServiceOptionManager, ServiceOptionManager>();
        services.AddScoped<IBookingManager, BookingManager>();
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IAnalyticsManager, AnalyticsManager>();

        return services;
    }
}
