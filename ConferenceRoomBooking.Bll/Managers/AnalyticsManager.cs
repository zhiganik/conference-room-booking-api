using ConferenceRoomBooking.Bll.Common.ManagerInterfaces;
using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;

namespace ConferenceRoomBooking.Bll.Managers;

public class AnalyticsManager(IAnalyticsRepository analyticsRepository) : IAnalyticsManager
{
    public Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
