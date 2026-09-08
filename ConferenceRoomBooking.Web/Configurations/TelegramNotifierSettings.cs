namespace ConferenceRoomBooking.Web.Configurations;

public class TelegramNotifierSettings
{
    public const string SectionName = "TelegramNotifier";

    public string BaseUrl { get; set; } = string.Empty;
}
