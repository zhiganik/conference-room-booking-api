namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetAvailableRoomsScenario : IRequestScenario
{
    public string Name => "GET /api/rooms/available";

    public Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, int requestIndex)
    {
        var start = DateTime.UtcNow.AddDays(7).Date.AddHours(9);
        var end = start.AddHours(2);
        var query = $"api/rooms/available?StartDate={Uri.EscapeDataString(start.ToString("o"))}" +
                    $"&EndDate={Uri.EscapeDataString(end.ToString("o"))}&Capacity=1";

        return httpClient.GetAsync(query);
    }
}
