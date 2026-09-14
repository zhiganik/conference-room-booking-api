using System.Data;

namespace ConferenceRoomBooking.Utils.Sql;

/// <summary>
/// Creates ADO.NET connections to an Azure SQL database, authenticated via Microsoft Entra ID.
/// Callers own the connection's lifetime (including opening it) — this only builds it.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Builds a new, unopened connection using the <c>DefaultConnection</c> connection string.
    /// </summary>
    IDbConnection CreateConnection();
}
