namespace ConferenceRoomBooking.LoadTesting;

public sealed record ComparisonGroup(string Name, IReadOnlyList<(int Parallelism, RequestStatistics Stats)> Results);
