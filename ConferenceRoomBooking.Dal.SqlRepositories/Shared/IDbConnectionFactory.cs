using System.Data;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Shared;

/// <summary>
/// Creates ADO.NET connections to the application database. Callers own the connection's lifetime
/// (including opening it) — this only builds it.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Builds a new, unopened connection using the <c>DefaultConnection</c> connection string.
    /// </summary>
    IDbConnection CreateConnection();
}
