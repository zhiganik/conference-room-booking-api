namespace ConferenceRoomBooking.Web.Dtos.Rooms;

public record UpdateRoomRequest(string Name, int Capacity, decimal BaseHourRate, List<int>? ServiceOptionIds);