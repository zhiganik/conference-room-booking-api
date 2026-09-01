namespace ConferenceRoomBooking.Web.Dtos.Booking;

public record CreateBookingRequest(int RoomId, DateTime StartTime, int DurationMinutes, List<int>? ServiceOptionIds);