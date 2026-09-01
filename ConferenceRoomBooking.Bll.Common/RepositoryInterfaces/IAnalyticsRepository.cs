using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;

public interface IAnalyticsRepository
{
    Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}
