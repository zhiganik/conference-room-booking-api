using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;
using ConferenceRoomBooking.Dal.SqlRepositories.Formatters;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Repositories;

public class SqlAnalyticsRepository(IDbConnectionFactory connectionFactory) : IAnalyticsRepository
{
    public Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
