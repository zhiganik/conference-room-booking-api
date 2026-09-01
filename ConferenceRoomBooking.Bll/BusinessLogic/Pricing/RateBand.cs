namespace ConferenceRoomBooking.Bll.BusinessLogic.Pricing;

public record RateBand(TimeSpan Start, TimeSpan End, decimal Multiplier);
