using System.Data;
using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Rooms.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Rooms;

public class RoomRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IRoomRepository
{
    public async Task<Room> CreateAsync(Room room, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Rooms_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", room.Name);
        command.Parameters.AddWithValue("@Capacity", room.Capacity);
        command.Parameters.AddWithValue("@BaseHourRate", room.BaseHourRate);
        AddServiceOptionIdsParameter(command, room.Services.Select(s => s.Id));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entities = await ReadRoomsAsync(reader, cancellationToken);

        return mapper.Map<Room>(entities.Single());
    }

    public async Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Rooms_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", roomId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var entities = await ReadRoomsAsync(reader, cancellationToken);

        return entities.Count == 0 ? null : mapper.Map<Room>(entities[0]);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Rooms_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", room.Id);
        command.Parameters.AddWithValue("@Name", room.Name);
        command.Parameters.AddWithValue("@Capacity", room.Capacity);
        command.Parameters.AddWithValue("@BaseHourRate", room.BaseHourRate);
        AddServiceOptionIdsParameter(command, room.Services.Select(s => s.Id));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SoftDeleteAsync(int roomId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Rooms_SoftDelete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", roomId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(int capacity, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Rooms_SearchAvailable", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Capacity", capacity);
        command.Parameters.AddWithValue("@StartTime", startTime);
        command.Parameters.AddWithValue("@EndTime", endTime);

        var results = new List<AvailableRoom>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new AvailableRoom
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                BaseHourlyRate = reader.GetDecimal(reader.GetOrdinal("BaseHourRate"))
            });
        }

        return results;
    }

    private static void AddServiceOptionIdsParameter(SqlCommand command, IEnumerable<int> serviceOptionIds)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));

        foreach (var id in serviceOptionIds)
        {
            table.Rows.Add(id);
        }

        var parameter = command.Parameters.AddWithValue("@ServiceOptionIds", table);
        parameter.SqlDbType = SqlDbType.Structured;
        parameter.TypeName = "MZhehistovskyi.IntIdList";
    }

    private static async Task<List<RoomEntity>> ReadRoomsAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        var rooms = new Dictionary<int, RoomEntity>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var roomId = reader.GetInt32(reader.GetOrdinal("Id"));
            if (!rooms.TryGetValue(roomId, out var room))
            {
                room = new RoomEntity
                {
                    Id = roomId,
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                    BaseHourRate = reader.GetDecimal(reader.GetOrdinal("BaseHourRate")),
                    CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                    IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                };
                rooms.Add(roomId, room);
            }

            var serviceOptionIdOrdinal = reader.GetOrdinal("ServiceOptionId");
            if (!await reader.IsDBNullAsync(serviceOptionIdOrdinal, cancellationToken))
            {
                room.ServiceOptions.Add(new ServiceOptionEntity
                {
                    Id = reader.GetInt32(serviceOptionIdOrdinal),
                    Name = reader.GetString(reader.GetOrdinal("ServiceOptionName")),
                    Price = reader.GetDecimal(reader.GetOrdinal("ServiceOptionPrice"))
                });
            }
        }

        return rooms.Values.ToList();
    }
}
