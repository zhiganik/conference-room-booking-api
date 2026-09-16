using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Creates a fresh, tagged, disposable service option on every call, unattached to any room.</summary>
public sealed class CreateServiceOptionScenario(RunMarker marker) : RequestScenarioBase(Actor.Admin)
{
    private static int _counter;

    public override string Name => "POST /api/ServiceOptions";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var n = Interlocked.Increment(ref _counter);
        var payload = new
        {
            Name = marker.ServiceOptionName($"Extra{n}"),
            Price = 5m
        };

        return SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/ServiceOptions", payload);
    }
}
