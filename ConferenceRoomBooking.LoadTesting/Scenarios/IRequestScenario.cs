namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public interface IRequestScenario
{
    string Name { get; }

    Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, int requestIndex);
}
