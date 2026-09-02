using ConferenceRoomBooking.Bll.Common.Analytics.Models;

namespace ConferenceRoomBooking.Bll.Common.Analytics;

/// <summary>
/// Business-facing reports on room and service performance. Currently a pure passthrough to
/// <see cref="IAnalyticsRepository"/> — there are no business rules to apply to read-only reports.
/// </summary>
public interface IAnalyticsManager
{
    /// <summary>Per-room booking count, revenue, average duration, and revenue rank.</summary>
    Task<IReadOnlyList<RoomPerformance>> GetRoomPerformanceAsync(CancellationToken cancellationToken);

    /// <summary>Per-service usage count, distinct rooms used in, revenue, and revenue rank.</summary>
    Task<IReadOnlyList<ServicePerformance>> GetServicePerformanceAsync(CancellationToken cancellationToken);
}
