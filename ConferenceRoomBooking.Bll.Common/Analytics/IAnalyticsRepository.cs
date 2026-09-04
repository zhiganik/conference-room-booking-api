using ConferenceRoomBooking.Bll.Common.Analytics.Models;

namespace ConferenceRoomBooking.Bll.Common.Analytics;

/// <summary>
/// Read-only reporting queries, backed by the <c>RoomPerformanceView</c> / <c>ServicePerformanceView</c>
/// SQL views.
/// </summary>
public interface IAnalyticsRepository
{
    /// <summary>Per-room booking count, revenue, average duration, and revenue rank.</summary>
    /// <returns>One row per room, ordered by revenue rank.</returns>
    Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken);

    /// <summary>Per-service usage count, distinct rooms used in, revenue, and revenue rank.</summary>
    /// <returns>One row per service option, ordered by revenue rank.</returns>
    Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}
