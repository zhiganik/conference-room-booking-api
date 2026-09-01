using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.ManagerInterfaces;

public interface IAnalyticsManager
{
    Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}
