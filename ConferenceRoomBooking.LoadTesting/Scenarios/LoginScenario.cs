using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Repeats the seed-user login. Safe and idempotent — issues a token, changes no data.</summary>
public sealed class LoginScenario(string email, string password) : RequestScenarioBase(Actor.Anonymous)
{
    public override string Name => "POST /api/auth/login";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var payload = new { Email = email, Password = password };
        return SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/auth/login", payload);
    }
}
