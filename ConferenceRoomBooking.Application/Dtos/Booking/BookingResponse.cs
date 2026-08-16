namespace ConferenceRoomBooking.Application.Dtos.Booking;

public record BookingResponse(
    int Id,
    int RoomId,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    List<BookedServiceOptionResponse> Services,
    BookingPriceBreakdownResponse PriceBreakdown,
    decimal TotalPrice);