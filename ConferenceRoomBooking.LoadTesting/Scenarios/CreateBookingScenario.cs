using System.Text;
using System.Text.Json;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class CreateBookingScenario(Guid roomId, DateTime baseDate) : IRequestScenario
{
    private const int DurationMinutes = 60;
    private const int OperatingWindowStartHour = 6;
    private const int SlotsPerDay = 17;

    public string Name => "POST /api/bookings";

    public Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, int requestIndex)
    {
        var dayOffset = requestIndex / SlotsPerDay;
        var hourOfDay = OperatingWindowStartHour + requestIndex % SlotsPerDay;
        var startTime = baseDate.AddDays(dayOffset).AddHours(hourOfDay);

        var payload = JsonSerializer.Serialize(new
        {
            RoomId = roomId,
            StartTime = startTime,
            DurationMinutes,
            ServiceOptionIds = (List<Guid>?)null
        });

        return httpClient.PostAsync("api/bookings", new StringContent(payload, Encoding.UTF8, "application/json"));
    }
}
