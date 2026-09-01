namespace ConferenceRoomBooking.Web.Dtos.Rooms;

public record AvailableRoomResponse(int Id, string Name, int Capacity, decimal BaseHourlyRate);