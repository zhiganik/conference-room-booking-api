using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Re-applies a fixture room's own values — idempotent, never touches real production rooms.</summary>
public sealed class UpdateRoomScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.Admin)
{
    public override string Name => "PUT /api/rooms/{roomId}";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var room = fixture.GetRandomRoom(Random.Shared);
        var payload = new
        {
            Name = room.Name,
            Capacity = room.Capacity,
            BaseHourRate = room.BaseHourlyRate,
            ServiceOptionIds = fixture.ServiceOptionIds
        };

        return SendAsync(httpClient, authContext, Actor, HttpMethod.Put, $"api/rooms/{room.Id}", payload);
    }
}
