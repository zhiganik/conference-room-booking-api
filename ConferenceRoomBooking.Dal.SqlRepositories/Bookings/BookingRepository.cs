using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Bookings;

public class BookingRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IBookingRepository
{
    public Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<bool> ExistsOverlappingAsync(int roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<bool> HasActiveForRoomAsync(int roomId, DateTime nowUtc, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
