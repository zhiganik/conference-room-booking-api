namespace ConferenceRoomBooking.Web.Dtos.Booking;

public record CreateBookingRequest(Guid RoomId, DateTime StartTime, int DurationMinutes, List<Guid>? ServiceOptionIds);