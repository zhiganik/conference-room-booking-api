using System.Net.Http.Json;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Dtos;

namespace ConferenceRoomBooking.LoadTesting.Fixtures;

/// <summary>
/// Creates the tagged, disposable rooms and service options a run mutates, via the real API
/// (admin token). These are the only entities PUT/DELETE/booking scenarios ever touch — existing
/// production data is never written to.
/// </summary>
public sealed class TestDataFixtureBuilder(HttpClient httpClient, AuthContext authContext, RunMarker marker)
{
    private const int RoomCount = 3;
    private const int ServiceOptionCount = 2;

    public async Task<TestDataFixture> BuildAsync()
    {
        var fixture = new TestDataFixture(marker);

        for (var i = 1; i <= ServiceOptionCount; i++)
        {
            var serviceOptionId = await CreateServiceOptionAsync(marker.ServiceOptionName(i.ToString()), price: 10m * i);
            fixture.AddServiceOption(serviceOptionId);
        }

        for (var i = 1; i <= RoomCount; i++)
        {
            var room = await CreateRoomAsync(marker.RoomName(i.ToString()), capacity: 2 + i, rate: 15m * i, fixture.ServiceOptionIds);
            fixture.AddRoom(room);
        }

        return fixture;
    }

    private async Task<Guid> CreateServiceOptionAsync(string name, decimal price)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/ServiceOptions")
        {
            Content = JsonContent.Create(new { Name = name, Price = price })
        };
        authContext.Apply(request, Actor.Admin);

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }

    private async Task<AvailableRoomDto> CreateRoomAsync(string name, int capacity, decimal rate, IReadOnlyList<Guid> serviceOptionIds)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/rooms")
        {
            Content = JsonContent.Create(new { Name = name, Capacity = capacity, BaseHourRate = rate, ServiceOptionIds = serviceOptionIds })
        };
        authContext.Apply(request, Actor.Admin);

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return new AvailableRoomDto(body.GetProperty("id").GetGuid(), name, capacity, rate);
    }
}
