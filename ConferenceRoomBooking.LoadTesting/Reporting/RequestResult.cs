namespace ConferenceRoomBooking.LoadTesting.Reporting;

public readonly record struct RequestResult(
    string ScenarioName, bool Success, int StatusCode, double ElapsedMs, int ThreadId, string? Error);
