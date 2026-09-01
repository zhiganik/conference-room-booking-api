namespace ConferenceRoomBooking.Application.Dtos.Booking;

public record CreateBookingRequest(int RoomId, DateTime StartTime, int DurationMinutes, List<int>? ServiceOptionIds);