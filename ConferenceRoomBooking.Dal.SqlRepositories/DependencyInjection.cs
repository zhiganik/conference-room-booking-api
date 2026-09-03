using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Dal.SqlRepositories.Analytics;
using ConferenceRoomBooking.Dal.SqlRepositories.Auth;
using ConferenceRoomBooking.Dal.SqlRepositories.Bookings;
using ConferenceRoomBooking.Dal.SqlRepositories.Rooms;
using ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceRoomBooking.Dal.SqlRepositories;

/// <summary>Registers the SQL-backed data access layer: the connection factory and every repository.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalSqlRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IServiceOptionRepository, ServiceOptionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

        return services;
    }
}
