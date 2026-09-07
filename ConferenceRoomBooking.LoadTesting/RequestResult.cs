namespace ConferenceRoomBooking.LoadTesting;

public readonly record struct RequestResult(bool Success, int StatusCode, double ElapsedMs, int ThreadId, string? Error);
