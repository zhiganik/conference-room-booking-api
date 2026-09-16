using System.Data;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using ConferenceRoomBooking.Utils.Sql;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.AlertSubscribers;

public class AlertSubscriberRepository(IDbConnectionFactory connectionFactory) : IAlertSubscriberRepository
{
    public async Task<bool> AddAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = new SqlCommand($"{DbSchema.Default}.sp_AlertSubscribers_Add", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add(new SqlParameter("@ChatId", SqlDbType.BigInt) { Value = chatId });

        var inserted = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return inserted is true;
    }

    public async Task RemoveAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = new SqlCommand($"{DbSchema.Default}.sp_AlertSubscribers_Remove", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add(new SqlParameter("@ChatId", SqlDbType.BigInt) { Value = chatId });

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<long>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = new SqlCommand($"{DbSchema.Default}.sp_AlertSubscribers_GetAll", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        List<long> results = [];
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        var chatIdOrdinal = reader.GetOrdinal("ChatId");

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            results.Add(reader.GetInt64(chatIdOrdinal));
        }

        return results;
    }
}
