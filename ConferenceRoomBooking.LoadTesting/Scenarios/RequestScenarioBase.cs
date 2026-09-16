using System.Net.Http.Json;
using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public abstract class RequestScenarioBase(Actor actor) : IRequestScenario
{
    public abstract string Name { get; }

    public Actor Actor { get; } = actor;

    public abstract Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex);

    protected static async Task<HttpResponseMessage> SendAsync(
        HttpClient httpClient, AuthContext authContext, Actor actor, HttpMethod method, string requestUri, object? body = null)
    {
        using var request = new HttpRequestMessage(method, requestUri);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        authContext.Apply(request, actor);
        return await httpClient.SendAsync(request);
    }
}
