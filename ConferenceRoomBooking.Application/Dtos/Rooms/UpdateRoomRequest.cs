namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record UpdateRoomRequest(string Name, int Capacity, decimal BaseHourRate, List<int>? ServiceOptionIds);