using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetRoomByIdScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.User)
{
    public override string Name => "GET /api/rooms/{roomId}";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var room = fixture.GetRandomRoom(Random.Shared);
        return SendAsync(httpClient, authContext, Actor, HttpMethod.Get, $"api/rooms/{room.Id}");
    }
}
