using System.Data;
using ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.TelegramNotifier.Dal.Shared;
using ConferenceRoomBooking.Utils.Sql;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.TelegramNotifier.Dal.AlertSubscribers;

public class AlertSubscriberRepository(IDbConnectionFactory connectionFactory) : IAlertSubscriberRepository
{
    public async Task<bool> AddAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_AlertSubscribers_Add", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@ChatId", chatId);

        var inserted = await command.ExecuteScalarAsync(cancellationToken);
        return inserted is true;
    }

    public async Task RemoveAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_AlertSubscribers_Remove", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@ChatId", chatId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_AlertSubscribers_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var results = new List<long>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var chatIdOrd = reader.GetOrdinal("ChatId");

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(reader.GetInt64(chatIdOrd));
        }

        return results;
    }
}
