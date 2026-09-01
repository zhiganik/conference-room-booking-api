using System.Data;
using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Auth.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Data.SqlClient;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Auth;

public class UserRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IUserRepository
{
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Users_Create", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@Role", user.Role);
        command.Parameters.AddWithValue("@CreatedAtUtc", user.CreatedAtUtc);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return mapper.Map<User>(MapEntity(reader));
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand("MZhehistovskyi.sp_Users_GetByEmail", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return mapper.Map<User>(MapEntity(reader));
    }

    private static UserEntity MapEntity(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Email = reader.GetString(reader.GetOrdinal("Email")),
        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        Role = reader.GetString(reader.GetOrdinal("Role")),
        CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
    };
}
