using System.Data;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Shared;

public class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private static readonly TokenCredential Credential = new DefaultAzureCredential();
    private static readonly string[] AzureSqlScope = ["https://database.windows.net/.default"];

    public IDbConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var connection = new SqlConnection(connectionString);

        connection.AccessTokenCallback = async (_, cancellationToken) =>
        {
            var token = await Credential.GetTokenAsync(new TokenRequestContext(AzureSqlScope), cancellationToken);
            return new SqlAuthenticationToken(token.Token, token.ExpiresOn);
        };

        return connection;
    }
}
