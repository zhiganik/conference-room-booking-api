namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>
/// Strategy for one kind of request (a specific endpoint + HTTP method + payload).
/// <see cref="LoadTestRunner"/> doesn't know or care which scenario it's running — it just
/// calls <see cref="ExecuteAsync"/> and times/records whatever comes back.
/// </summary>
public interface IRequestScenario
{
    /// <summary>Shown in the report header.</summary>
    string Name { get; }

    /// <param name="httpClient">The shared, already-authenticated client.</param>
    /// <param name="requestIndex">0-based index of this request within the run, in case a scenario wants to vary its payload per call.</param>
    Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, int requestIndex);
}
