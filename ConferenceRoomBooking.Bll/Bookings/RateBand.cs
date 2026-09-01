namespace ConferenceRoomBooking.Bll.Bookings;

public record RateBand(TimeSpan Start, TimeSpan End, decimal Multiplier);
