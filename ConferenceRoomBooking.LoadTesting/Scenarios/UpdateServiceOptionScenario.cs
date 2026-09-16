using System.Net.Http.Json;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>Re-reads and re-applies a fixture service option's own price — idempotent, fixture-scoped.</summary>
public sealed class UpdateServiceOptionScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.Admin)
{
    public override string Name => "PUT /api/ServiceOptions/{serviceOptionId}";

    public override async Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        var serviceOptionId = fixture.GetRandomServiceOptionId(Random.Shared);

        using var current = await SendAsync(httpClient, authContext, Actor, HttpMethod.Get, $"api/ServiceOptions/{serviceOptionId}");
        current.EnsureSuccessStatusCode();
        var body = await current.Content.ReadFromJsonAsync<JsonElement>();

        var payload = new
        {
            Name = body.GetProperty("name").GetString(),
            Price = body.GetProperty("price").GetDecimal()
        };

        return await SendAsync(httpClient, authContext, Actor, HttpMethod.Put, $"api/ServiceOptions/{serviceOptionId}", payload);
    }
}
