using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Registers a fresh, tagged synthetic user on every call. Emails are recorded on the
/// fixture so the final report can list them for manual cleanup.</summary>
public sealed class RegisterUserScenario(RunMarker marker, TestDataFixture fixture) : RequestScenarioBase(Actor.Anonymous)
{
    private static int _counter;

    public override string Name => "POST /api/auth/register";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var n = Interlocked.Increment(ref _counter);
        var email = marker.UserEmail(n.ToString());
        fixture.RegisterCreatedUser(email);

        var payload = new { Email = email, Password = "LoadTest@12345!" };
        return SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/auth/register", payload);
    }
}
