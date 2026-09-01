using ConferenceRoomBooking.Bll.Common.Bookings.Models;

namespace ConferenceRoomBooking.Bll.Common.Bookings;

public interface IBookingRepository
{
    Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> ExistsOverlappingAsync(int roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
    Task<bool> HasActiveForRoomAsync(int roomId, DateTime nowUtc, CancellationToken cancellationToken);
}
