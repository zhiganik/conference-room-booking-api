namespace ConferenceRoomBooking.Web.Configurations;

public class HourlyBookingReportSettings
{
    public const string SectionName = "HourlyBookingReport";

    public int IntervalMinutes { get; set; } = 60;
}
