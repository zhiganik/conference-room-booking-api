namespace ConferenceRoomBooking.LoadTesting.Dtos;

public sealed record AvailableRoomDto(Guid Id, string Name, int Capacity, decimal BaseHourlyRate);
