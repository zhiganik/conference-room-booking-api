using ConferenceRoomBooking.Application.Dtos.Booking;

namespace ConferenceRoomBooking.Application.Orchestrators.Booking;

public interface IBookingOrchestrator
{
    public Task<BookingResponse> BookRoom(CreateBookingRequest request, CancellationToken cancellationToken);
}