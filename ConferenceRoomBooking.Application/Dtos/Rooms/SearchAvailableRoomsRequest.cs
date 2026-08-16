namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record SearchAvailableRoomsRequest(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, int Capacity);