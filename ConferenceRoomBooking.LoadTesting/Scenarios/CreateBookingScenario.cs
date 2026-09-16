using System.Net.Http.Json;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>
/// Books a random fixture room at a thread-safe, unique slot so concurrent dispatches never
/// collide. Only ever targets synthetic rooms created by <see cref="TestDataFixtureBuilder"/> —
/// never a real production room.
/// </summary>
public sealed class CreateBookingScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.User)
{
    private const int DurationMinutes = 60;
    private const int OperatingWindowStartHour = 6;
    private const int SlotsPerDay = 17;
    private static readonly DateTime BaseDate = DateTime.UtcNow.Date.AddDays(30 + Random.Shared.Next(0, 5000));

    public override string Name => "POST /api/bookings";

    public override async Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var room = fixture.GetRandomRoom(Random.Shared);
        var slot = fixture.NextSlotOffset(room.Id);
        var dayOffset = slot / SlotsPerDay;
        var hourOfDay = OperatingWindowStartHour + slot % SlotsPerDay;
        var startTime = BaseDate.AddDays(dayOffset).AddHours(hourOfDay);

        var payload = new
        {
            RoomId = room.Id,
            StartTime = startTime,
            DurationMinutes,
            ServiceOptionIds = (List<Guid>?)null
        };

        var response = await SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/bookings", payload);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (body.TryGetProperty("id", out var idProperty))
            {
                fixture.RegisterCreatedBooking(idProperty.GetGuid());
            }
        }

        return response;
    }
}
