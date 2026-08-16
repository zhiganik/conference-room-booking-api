namespace ConferenceRoomBooking.Application.BusinessLogic.Pricing;

public record RateBand(TimeSpan Start, TimeSpan End, decimal Multiplier);