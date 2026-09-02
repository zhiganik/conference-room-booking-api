using System.Data;
using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Bookings.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Bookings;

public class BookingRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IBookingRepository
{
    public async Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Bookings_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@RoomId", booking.RoomId);
        command.Parameters.AddWithValue("@RoomName", booking.RoomName);
        command.Parameters.AddWithValue("@UserId", booking.UserId);
        command.Parameters.AddWithValue("@StartTime", booking.StartTime);
        command.Parameters.AddWithValue("@EndTime", booking.EndTime);
        command.Parameters.AddWithValue("@BaseRoomCost", booking.BaseRoomCost);
        command.Parameters.AddWithValue("@ServicesCost", booking.ServicesCost);
        command.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
        AddServiceOptionsParameter(command, booking.Services);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (int)result!;
    }

    public async Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Bookings_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", bookingId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entities = await ReadBookingsAsync(reader, cancellationToken);

        return entities.Count == 0 ? null : mapper.Map<Booking>(entities[0]);
    }

    public async Task<IReadOnlyList<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Bookings_GetByUserId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entities = await ReadBookingsAsync(reader, cancellationToken);

        return entities.Select(mapper.Map<Booking>).ToList();
    }

    public async Task<bool> ExistsOverlappingAsync(int roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Bookings_ExistsOverlapping", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@RoomId", roomId);
        command.Parameters.AddWithValue("@StartTime", startTime);
        command.Parameters.AddWithValue("@EndTime", endTime);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (bool)result!;
    }

    public async Task<bool> HasActiveForRoomAsync(int roomId, DateTime nowUtc, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Bookings_HasActiveForRoom", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@RoomId", roomId);
        command.Parameters.AddWithValue("@NowUtc", nowUtc);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (bool)result!;
    }

    private static void AddServiceOptionsParameter(SqlCommand command, IEnumerable<BookedServiceOption> services)
    {
        var table = new DataTable();
        table.Columns.Add("ServiceOptionId", typeof(int));
        table.Columns.Add("ServiceOptionName", typeof(string));
        table.Columns.Add("PriceAtBooking", typeof(decimal));

        foreach (var service in services)
        {
            table.Rows.Add(service.ServiceOptionId, service.Name, service.PriceAtBooking);
        }

        var parameter = command.Parameters.AddWithValue("@ServiceOptions", table);
        parameter.SqlDbType = SqlDbType.Structured;
        parameter.TypeName = "MZhehistovskyi.BookingServiceOptionList";
    }

    private static async Task<List<BookingEntity>> ReadBookingsAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var bookings = new List<BookingEntity>();
        var bookingsById = new Dictionary<int, BookingEntity>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var bookingId = reader.GetInt32(reader.GetOrdinal("Id"));
            if (!bookingsById.TryGetValue(bookingId, out var booking))
            {
                booking = new BookingEntity
                {
                    Id = bookingId,
                    RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    BaseRoomCost = reader.GetDecimal(reader.GetOrdinal("BaseRoomCost")),
                    ServicesCost = reader.GetDecimal(reader.GetOrdinal("ServicesCost")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                };
                bookingsById.Add(bookingId, booking);
                bookings.Add(booking);
            }

            var serviceOptionIdOrdinal = reader.GetOrdinal("ServiceOptionId");
            if (!await reader.IsDBNullAsync(serviceOptionIdOrdinal, cancellationToken))
            {
                booking.ServiceOptions.Add(new BookingServiceOptionEntity
                {
                    BookingId = bookingId,
                    ServiceOptionId = reader.GetInt32(serviceOptionIdOrdinal),
                    ServiceOptionName = reader.GetString(reader.GetOrdinal("ServiceOptionName")),
                    PriceAtBooking = reader.GetDecimal(reader.GetOrdinal("PriceAtBooking"))
                });
            }
        }

        return bookings;
    }
}
