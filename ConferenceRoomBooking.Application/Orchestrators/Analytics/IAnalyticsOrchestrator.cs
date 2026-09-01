using ConferenceRoomBooking.Application.Dtos.Analytics;

namespace ConferenceRoomBooking.Application.Orchestrators.Analytics;

public interface IAnalyticsOrchestrator
{
    Task<IReadOnlyList<RoomPerformanceResponse>> GetRoomPerformanceAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ServicePerformanceResponse>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}