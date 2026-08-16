namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record CreateRoomRequest(string Name, int Capacity, decimal BaseHourRate, List<int>? ServiceOptionIds);