using System.Data;
using AutoMapper;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions;

public class ServiceOptionRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IServiceOptionRepository
{
    public async Task<ServiceOption> CreateAsync(ServiceOption serviceOption, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", serviceOption.Name);
        command.Parameters.AddWithValue("@Price", serviceOption.Price);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return mapper.Map<ServiceOption>(MapEntity(reader));
    }

    public async Task<ServiceOption?> GetByIdAsync(Guid serviceOptionId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_GetById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", serviceOptionId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return mapper.Map<ServiceOption>(MapEntity(reader));
    }

    public async Task<ServiceOption?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_GetByName", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", name);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return mapper.Map<ServiceOption>(MapEntity(reader));
    }

    public async Task<IReadOnlyList<ServiceOption>> GetByIdsAsync(IReadOnlyCollection<Guid> serviceOptionIds, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_GetByIds", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        var idsParameter = command.Parameters.AddWithValue("@Ids", BuildGuidIdListTable(serviceOptionIds));
        idsParameter.SqlDbType = SqlDbType.Structured;
        idsParameter.TypeName = $"{DbSchema.Name}.GuidIdList";

        var results = new List<ServiceOption>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(mapper.Map<ServiceOption>(MapEntity(reader)));
        }

        return results;
    }

    public async Task UpdateAsync(ServiceOption serviceOption, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_Update", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", serviceOption.Id);
        command.Parameters.AddWithValue("@Name", serviceOption.Name);
        command.Parameters.AddWithValue("@Price", serviceOption.Price);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid serviceOptionId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_Delete", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Id", serviceOptionId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_Search", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", (object?)name ?? DBNull.Value);

        var results = new List<ServiceOption>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(mapper.Map<ServiceOption>(MapEntity(reader)));
        }

        return results;
    }

    public async Task<bool> IsInUseByRoomAsync(Guid serviceOptionId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_IsInUseByRoom", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@ServiceOptionId", serviceOptionId);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (bool)result!;
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludingId, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand($"{DbSchema.Name}.sp_ServiceOptions_ExistsByName", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ExcludingId", (object?)excludingId ?? DBNull.Value);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (bool)result!;
    }

    private static DataTable BuildGuidIdListTable(IReadOnlyCollection<Guid> ids)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));

        foreach (var id in ids)
        {
            table.Rows.Add(id);
        }

        return table;
    }

    private static ServiceOptionEntity MapEntity(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
    };
}
