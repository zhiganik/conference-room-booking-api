using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Analytics.Models;

namespace ConferenceRoomBooking.Bll.Analytics;

public class AnalyticsManager(IAnalyticsRepository analyticsRepository) : IAnalyticsManager
{
    public Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken) =>
        analyticsRepository.GetRoomPerformanceAsync(cancellationToken);

    public Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken) =>
        analyticsRepository.GetServicePerformanceAsync(cancellationToken);
}
