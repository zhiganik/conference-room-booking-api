using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;

namespace ConferenceRoomBooking.Bll.Bookings;

public class BookingManager(
    IBookingRepository bookingRepository,
    IRoomRepository roomRepository,
    IServiceOptionRepository serviceOptionRepository,
    IRentalPriceCalculator priceCalculator,
    IUserContext userContext,
    IRoomBookingLock roomBookingLock) : IBookingManager
{
    public Task<Booking> CreateAsync(int roomId, DateTime startTime, int durationMinutes, List<int>? serviceOptionIds, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Booking> GetByIdAsync(int bookingId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<Booking>> GetByUserAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
