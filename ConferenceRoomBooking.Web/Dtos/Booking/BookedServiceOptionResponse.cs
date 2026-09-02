namespace ConferenceRoomBooking.Web.Dtos.Booking;

public record BookedServiceOptionResponse(Guid ServiceOptionId, string Name, decimal PriceAtBooking);