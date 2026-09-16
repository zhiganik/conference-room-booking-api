using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetServiceOptionByIdScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.User)
{
    public override string Name => "GET /api/ServiceOptions/{serviceOptionId}";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var serviceOptionId = fixture.GetRandomServiceOptionId(Random.Shared);
        return SendAsync(httpClient, authContext, Actor, HttpMethod.Get, $"api/ServiceOptions/{serviceOptionId}");
    }
}
