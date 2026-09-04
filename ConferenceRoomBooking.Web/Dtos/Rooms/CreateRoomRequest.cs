namespace ConferenceRoomBooking.Web.Dtos.Rooms;

public record CreateRoomRequest(string Name, int Capacity, decimal BaseHourRate, List<Guid>? ServiceOptionIds);