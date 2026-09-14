using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Reports;
using ConferenceRoomBooking.Bll.Common.Reports.Models;

namespace ConferenceRoomBooking.Bll.Reports;

public class BookingReportManager(IBookingRepository bookingRepository) : IBookingReportManager
{
    public async Task<HourlyBookingReport> GenerateHourlyReportAsync(
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        CancellationToken cancellationToken)
    {
        var bookings = await bookingRepository.GetCreatedBetweenAsync(periodStartUtc, periodEndUtc, cancellationToken);

        return new HourlyBookingReport
        {
            PeriodStartUtc = periodStartUtc,
            PeriodEndUtc = periodEndUtc,
            GeneratedAtUtc = DateTime.UtcNow,
            TotalBookings = bookings.Count,
            TotalRevenue = bookings.Sum(b => b.TotalPrice),
            Bookings = bookings
        };
    }
}
