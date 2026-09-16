using System.Net.Http.Headers;

namespace ConferenceRoomBooking.LoadTesting.Auth;

/// <summary>
/// Holds pre-resolved bearer tokens and applies the right one per-request based on the
/// scenario's <see cref="Actor"/>. Never mutates HttpClient.DefaultRequestHeaders, so the
/// shared HttpClient stays safe under concurrent, differently-authenticated requests.
/// </summary>
public sealed class AuthContext(string userToken, string adminToken)
{
    public void Apply(HttpRequestMessage request, Actor actor)
    {
        request.Headers.Authorization = actor switch
        {
            Actor.Anonymous => null,
            Actor.User => new AuthenticationHeaderValue("Bearer", userToken),
            Actor.Admin => new AuthenticationHeaderValue("Bearer", adminToken),
            _ => throw new ArgumentOutOfRangeException(nameof(actor), actor, null)
        };
    }
}
