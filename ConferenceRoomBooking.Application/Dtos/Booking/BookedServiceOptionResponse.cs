namespace ConferenceRoomBooking.Application.Dtos.Booking;

public record BookedServiceOptionResponse(int ServiceOptionId, string Name, decimal PriceAtBooking);