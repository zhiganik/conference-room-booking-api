using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.ManagerInterfaces;

public interface IBookingManager
{
    Task<Booking> CreateAsync(int roomId, DateTime startTime, int durationMinutes, List<int>? serviceOptionIds, CancellationToken cancellationToken);
    Task<Booking> GetByIdAsync(int bookingId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetByUserAsync(CancellationToken cancellationToken);
}
