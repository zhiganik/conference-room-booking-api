using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Analytics.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Analytics;

public class AnalyticsRepository(IDbConnectionFactory connectionFactory) : IAnalyticsRepository
{
    public Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
