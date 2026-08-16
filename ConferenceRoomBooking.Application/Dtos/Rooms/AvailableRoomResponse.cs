namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record AvailableRoomResponse(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, int Capacity);