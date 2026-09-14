using ConferenceRoomBooking.Bll.Common.Reports.Models;

namespace ConferenceRoomBooking.Bll.Common.Reports;

/// <summary>
/// Builds reports summarizing booking activity over a time window.
/// </summary>
public interface IBookingReportManager
{
    /// <summary>Summarizes every booking created within <paramref name="periodStartUtc"/> (inclusive)
    /// and <paramref name="periodEndUtc"/> (exclusive).</summary>
    Task<HourlyBookingReport> GenerateHourlyReportAsync(
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        CancellationToken cancellationToken);
}
