using System.Data;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Shared;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
