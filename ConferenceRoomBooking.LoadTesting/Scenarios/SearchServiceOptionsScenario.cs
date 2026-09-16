using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class SearchServiceOptionsScenario() : RequestScenarioBase(Actor.User)
{
    public override string Name => "GET /api/ServiceOptions";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex) =>
        SendAsync(httpClient, authContext, Actor, HttpMethod.Get, "api/ServiceOptions");
}
