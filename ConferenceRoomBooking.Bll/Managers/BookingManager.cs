using ConferenceRoomBooking.Bll.BusinessLogic.Pricing;
using ConferenceRoomBooking.Bll.Common.Abstractions;
using ConferenceRoomBooking.Bll.Common.ManagerInterfaces;
using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;
using ConferenceRoomBooking.Bll.Services;

namespace ConferenceRoomBooking.Bll.Managers;

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
