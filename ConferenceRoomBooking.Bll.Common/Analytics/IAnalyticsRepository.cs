using ConferenceRoomBooking.Bll.Common.Analytics.Models;

namespace ConferenceRoomBooking.Bll.Common.Analytics;

public interface IAnalyticsRepository
{
    Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}
