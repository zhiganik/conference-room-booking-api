namespace ConferenceRoomBooking.LoadTesting;

/// <summary>The outcome of a single request: how it went, how long it took, and which thread handled it.</summary>
public readonly record struct RequestResult(bool Success, int StatusCode, double ElapsedMs, int ThreadId, string? Error);
