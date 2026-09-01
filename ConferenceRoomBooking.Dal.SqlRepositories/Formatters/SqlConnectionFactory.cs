using System.Data;
using Microsoft.Extensions.Configuration;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Formatters;

public class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => throw new NotImplementedException();
}
