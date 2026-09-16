using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetRoomPerformanceScenario() : RequestScenarioBase(Actor.Admin)
{
    public override string Name => "GET /api/analytics/room-performance";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex) =>
        SendAsync(httpClient, authContext, Actor, HttpMethod.Get, "api/analytics/room-performance");
}
