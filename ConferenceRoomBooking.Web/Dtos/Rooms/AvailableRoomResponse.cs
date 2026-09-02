namespace ConferenceRoomBooking.Web.Dtos.Rooms;

public record AvailableRoomResponse(Guid Id, string Name, int Capacity, decimal BaseHourlyRate);