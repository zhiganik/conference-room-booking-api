using System.Net.Http.Json;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>
/// Creates its own fresh, unattached tagged service option and immediately hard-deletes it, so
/// DELETE is exercised without risking a 409 (service options attached to a room can't be
/// deleted) and without leaving anything behind to clean up. Two physical HTTP calls happen per
/// dispatch; only the DELETE call's timing is reported.
/// </summary>
public sealed class DeleteServiceOptionScenario(RunMarker marker) : RequestScenarioBase(Actor.Admin)
{
    private static int _counter;

    public override string Name => "DELETE /api/ServiceOptions/{serviceOptionId}";

    public override async Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var n = Interlocked.Increment(ref _counter);
        var payload = new
        {
            Name = marker.ServiceOptionName($"Del{n}"),
            Price = 5m
        };

        using var createResponse = await SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/ServiceOptions", payload);
        createResponse.EnsureSuccessStatusCode();
        var body = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var serviceOptionId = body.GetProperty("id").GetGuid();

        return await SendAsync(httpClient, authContext, Actor, HttpMethod.Delete, $"api/ServiceOptions/{serviceOptionId}");
    }
}
