namespace ConferenceRoomBooking.LoadTesting;

public sealed record AvailableRoomDto(Guid Id, string Name, int Capacity, decimal BaseHourlyRate);
