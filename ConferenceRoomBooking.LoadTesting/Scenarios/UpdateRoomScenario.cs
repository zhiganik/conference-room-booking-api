using System.Text;
using System.Text.Json;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class UpdateRoomScenario(Guid roomId, string roomName, int capacity, decimal baseHourRate) : IRequestScenario
{
    public string Name => "PUT /api/rooms/{roomId}";

    public Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, int requestIndex)
    {
        var payload = JsonSerializer.Serialize(new
        {
            Name = roomName,
            Capacity = capacity,
            BaseHourRate = baseHourRate,
            ServiceOptionIds = (List<Guid>?)null
        });

        return httpClient.PutAsync($"api/rooms/{roomId}", new StringContent(payload, Encoding.UTF8, "application/json"));
    }
}
