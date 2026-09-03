using System.Data;
using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Analytics.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Analytics;

public class AnalyticsRepository(IDbConnectionFactory connectionFactory) : IAnalyticsRepository
{
    public async Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Analytics_GetRoomPerformance", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var results = new List<RoomPerformance>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var roomIdOrd = reader.GetOrdinal("RoomId");
        var roomNameOrd = reader.GetOrdinal("RoomName");
        var totalBookingsOrd = reader.GetOrdinal("TotalBookings");
        var totalRevenueOrd = reader.GetOrdinal("TotalRevenue");
        var avgBookingDurationMinutesOrd = reader.GetOrdinal("AvgBookingDurationMinutes");
        var revenueRankOrd = reader.GetOrdinal("RevenueRank");

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new RoomPerformance(
                reader.GetGuid(roomIdOrd),
                reader.GetString(roomNameOrd),
                reader.GetInt32(totalBookingsOrd),
                reader.GetDecimal(totalRevenueOrd),
                reader.GetDecimal(avgBookingDurationMinutesOrd),
                reader.GetInt32(revenueRankOrd)));
        }

        return results;
    }

    public async Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Analytics_GetServicePerformance", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var results = new List<ServicePerformance>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var idOrd = reader.GetOrdinal("Id");
        var nameOrd = reader.GetOrdinal("Name");
        var timesSelectedOrd = reader.GetOrdinal("TimesSelected");
        var distinctRoomsUsedInOrd = reader.GetOrdinal("DistinctRoomsUsedIn");
        var totalRevenueOrd = reader.GetOrdinal("TotalRevenue");
        var revenueRankOrd = reader.GetOrdinal("RevenueRank");

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new ServicePerformance(
                reader.GetGuid(idOrd),
                reader.GetString(nameOrd),
                reader.GetInt32(timesSelectedOrd),
                reader.GetInt32(distinctRoomsUsedInOrd),
                reader.GetDecimal(totalRevenueOrd),
                reader.GetInt32(revenueRankOrd)));
        }

        return results;
    }
}
