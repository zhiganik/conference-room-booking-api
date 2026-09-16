using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetAvailableRoomsScenario() : RequestScenarioBase(Actor.User)
{
    public override string Name => "GET /api/rooms/available";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var start = DateTime.UtcNow.AddDays(7).Date.AddHours(9);
        var end = start.AddHours(2);
        var query = $"api/rooms/available?StartDate={Uri.EscapeDataString(start.ToString("o"))}" +
                    $"&EndDate={Uri.EscapeDataString(end.ToString("o"))}&Capacity=1";

        return SendAsync(httpClient, authContext, Actor, HttpMethod.Get, query);
    }
}
