using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Creates a fresh, tagged, disposable room on every call. Never touches the shared fixture pool.</summary>
public sealed class CreateRoomScenario(RunMarker marker) : RequestScenarioBase(Actor.Admin)
{
    private static int _counter;

    public override string Name => "POST /api/rooms";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var n = Interlocked.Increment(ref _counter);
        var payload = new
        {
            Name = marker.RoomName($"Extra{n}"),
            Capacity = 2,
            BaseHourRate = 10m,
            ServiceOptionIds = (List<Guid>?)null
        };

        return SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/rooms", payload);
    }
}
