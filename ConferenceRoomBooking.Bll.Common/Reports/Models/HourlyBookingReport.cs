using ConferenceRoomBooking.Bll.Common.Bookings.Models;

namespace ConferenceRoomBooking.Bll.Common.Reports.Models;

public class HourlyBookingReport
{
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    public DateTime GeneratedAtUtc { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public IReadOnlyCollection<Booking> Bookings { get; set; } = [];
}
