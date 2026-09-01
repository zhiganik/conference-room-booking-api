using System.Data;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Formatters;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
