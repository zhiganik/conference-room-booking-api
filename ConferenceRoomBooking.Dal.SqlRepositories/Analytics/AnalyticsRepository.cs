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

        await using var command = new SqlCommand("MZhehistovskyi.sp_Analytics_GetRoomPerformance", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var results = new List<RoomPerformance>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new RoomPerformance(
                reader.GetGuid(reader.GetOrdinal("RoomId")),
                reader.GetString(reader.GetOrdinal("RoomName")),
                reader.GetInt32(reader.GetOrdinal("TotalBookings")),
                reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                reader.GetDecimal(reader.GetOrdinal("AvgBookingDurationMinutes")),
                reader.GetInt32(reader.GetOrdinal("RevenueRank"))));
        }

        return results;
    }

    public async Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Analytics_GetServicePerformance", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var results = new List<ServicePerformance>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new ServicePerformance(
                reader.GetGuid(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetInt32(reader.GetOrdinal("TimesSelected")),
                reader.GetInt32(reader.GetOrdinal("DistinctRoomsUsedIn")),
                reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                reader.GetInt32(reader.GetOrdinal("RevenueRank"))));
        }

        return results;
    }
}
