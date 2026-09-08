namespace ConferenceRoomBooking.Bll.Common.Reports;

/// <summary>
/// Orchestrates the hourly booking report workflow: build the report, persist it, notify.
/// Depends only on <see cref="IBookingReportManager"/>, <see cref="IBookingReportStorage"/>, and
/// <see cref="Notifications.INotifier"/> — no storage/notification implementation details here.
/// </summary>
public interface IHourlyBookingReportPublisher
{
    Task PublishAsync(DateTime periodStartUtc, DateTime periodEndUtc, CancellationToken cancellationToken);
}
