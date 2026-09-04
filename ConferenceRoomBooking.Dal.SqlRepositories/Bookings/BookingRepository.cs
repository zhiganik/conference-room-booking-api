using System.Data;
using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Bookings.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Bookings;

public class BookingRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IBookingRepository
{
    /// <summary>THROW error number raised by sp_Bookings_Create when the UPDLOCK/HOLDLOCK overlap
    /// check finds a conflicting booking. Keep in sync with that procedure.</summary>
    private const int RoomUnavailableErrorNumber = 50001;

    public async Task<Guid> CreateAsync(Booking booking, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Bookings_Create", connection)
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

        try
        {
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return (Guid)result!;
        }
        catch (SqlException ex) when (ex.Number == RoomUnavailableErrorNumber)
        {
            throw new RoomUnavailableException("Room is already booked during the requested time window.");
        }
    }

    public async Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Bookings_GetById", connection)
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

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Bookings_GetByUserId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entities = await ReadBookingsAsync(reader, cancellationToken);

        return entities.Select(mapper.Map<Booking>).ToList();
    }

    public async Task<bool> ExistsOverlappingAsync(Guid roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Bookings_ExistsOverlapping", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@RoomId", roomId);
        command.Parameters.AddWithValue("@StartTime", startTime);
        command.Parameters.AddWithValue("@EndTime", endTime);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (bool)result!;
    }

    public async Task<bool> HasActiveForRoomAsync(Guid roomId, DateTime nowUtc, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_Bookings_HasActiveForRoom", connection)
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
        table.Columns.Add("ServiceOptionId", typeof(Guid));
        table.Columns.Add("ServiceOptionName", typeof(string));
        table.Columns.Add("PriceAtBooking", typeof(decimal));

        foreach (var service in services)
        {
            table.Rows.Add(service.ServiceOptionId, service.Name, service.PriceAtBooking);
        }

        var parameter = command.Parameters.AddWithValue("@ServiceOptions", table);
        parameter.SqlDbType = SqlDbType.Structured;
        parameter.TypeName = $"{DbSchema.Name}.BookingServiceOptionList";
    }

    private static async Task<List<BookingEntity>> ReadBookingsAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var bookings = new List<BookingEntity>();
        var bookingsById = new Dictionary<Guid, BookingEntity>();

        var idOrd = reader.GetOrdinal("Id");
        var roomIdOrd = reader.GetOrdinal("RoomId");
        var roomNameOrd = reader.GetOrdinal("RoomName");
        var userIdOrd = reader.GetOrdinal("UserId");
        var startTimeOrd = reader.GetOrdinal("StartTime");
        var endTimeOrd = reader.GetOrdinal("EndTime");
        var baseRoomCostOrd = reader.GetOrdinal("BaseRoomCost");
        var servicesCostOrd = reader.GetOrdinal("ServicesCost");
        var totalPriceOrd = reader.GetOrdinal("TotalPrice");
        var createdAtUtcOrd = reader.GetOrdinal("CreatedAtUtc");
        var serviceOptionIdOrdinal = reader.GetOrdinal("ServiceOptionId");
        var serviceOptionNameOrd = reader.GetOrdinal("ServiceOptionName");
        var priceAtBookingOrd = reader.GetOrdinal("PriceAtBooking");

        while (await reader.ReadAsync(cancellationToken))
        {
            var bookingId = reader.GetGuid(idOrd);
            if (!bookingsById.TryGetValue(bookingId, out var booking))
            {
                booking = new BookingEntity
                {
                    Id = bookingId,
                    RoomId = reader.GetGuid(roomIdOrd),
                    RoomName = reader.GetString(roomNameOrd),
                    UserId = reader.GetGuid(userIdOrd),
                    StartTime = reader.GetDateTime(startTimeOrd),
                    EndTime = reader.GetDateTime(endTimeOrd),
                    BaseRoomCost = reader.GetDecimal(baseRoomCostOrd),
                    ServicesCost = reader.GetDecimal(servicesCostOrd),
                    TotalPrice = reader.GetDecimal(totalPriceOrd),
                    CreatedAtUtc = reader.GetDateTime(createdAtUtcOrd)
                };
                bookingsById.Add(bookingId, booking);
                bookings.Add(booking);
            }

            if (!await reader.IsDBNullAsync(serviceOptionIdOrdinal, cancellationToken))
            {
                booking.ServiceOptions.Add(new BookingServiceOptionEntity
                {
                    BookingId = bookingId,
                    ServiceOptionId = reader.GetGuid(serviceOptionIdOrdinal),
                    ServiceOptionName = reader.GetString(serviceOptionNameOrd),
                    PriceAtBooking = reader.GetDecimal(priceAtBookingOrd)
                });
            }
        }

        return bookings;
    }
}
