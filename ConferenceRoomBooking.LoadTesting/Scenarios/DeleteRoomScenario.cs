using System.Net.Http.Json;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>
/// Creates its own fresh tagged room and immediately soft-deletes it, so DELETE is exercised
/// without ever touching the shared fixture pool or a real production room. Two physical HTTP
/// calls happen per dispatch; only the DELETE call's timing is reported.
/// </summary>
public sealed class DeleteRoomScenario(RunMarker marker) : RequestScenarioBase(Actor.Admin)
{
    private static int _counter;

    public override string Name => "DELETE /api/rooms/{roomId}";

    public override async Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var n = Interlocked.Increment(ref _counter);
        var payload = new
        {
            Name = marker.RoomName($"Del{n}"),
            Capacity = 2,
            BaseHourRate = 10m,
            ServiceOptionIds = (List<Guid>?)null
        };

        using var createResponse = await SendAsync(httpClient, authContext, Actor, HttpMethod.Post, "api/rooms", payload);
        createResponse.EnsureSuccessStatusCode();
        var body = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var roomId = body.GetProperty("id").GetGuid();

        return await SendAsync(httpClient, authContext, Actor, HttpMethod.Delete, $"api/rooms/{roomId}");
    }
}
