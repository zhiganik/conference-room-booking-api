namespace ConferenceRoomBooking.Web.Dtos.Booking;

public record BookingResponse(
    Guid Id,
    Guid RoomId,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    List<BookedServiceOptionResponse> Services,
    BookingPriceBreakdownResponse PriceBreakdown,
    decimal TotalPrice);