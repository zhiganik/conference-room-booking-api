using ConferenceRoomBooking.Application.Dtos.Booking;

namespace ConferenceRoomBooking.Application.Orchestrators.Booking;

public interface IBookingOrchestrator
{
    Task<BookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken);
    Task<BookingResponse> GetByIdAsync(int bookingId, CancellationToken cancellationToken);
}