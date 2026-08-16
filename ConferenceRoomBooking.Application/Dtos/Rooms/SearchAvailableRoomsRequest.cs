namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record SearchAvailableRoomsRequest(DateTime StartDate, DateTime EndDate, int Capacity);