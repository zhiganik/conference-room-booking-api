using ConferenceRoomBooking.Bll.Common.Bookings.Models;

namespace ConferenceRoomBooking.Bll.Common.Bookings;

/// <summary>
/// Persistence for <c>Bookings</c> and their booked services (<c>BookingServiceOptions</c>).
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Inserts a new booking together with its booked services, in one transaction.
    /// </summary>
    /// <param name="booking">The booking to create. <see cref="Booking.Services"/> determines the
    /// <c>BookingServiceOptions</c> rows written alongside it.</param>
    /// <returns>The id of the created booking.</returns>
    Task<Guid> CreateAsync(Booking booking, CancellationToken cancellationToken);

    /// <summary>Looks up a booking by id, including its booked services.</summary>
    /// <returns>The matching booking, or <see langword="null"/> if no such booking exists.</returns>
    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken);

    /// <summary>Retrieves every booking made by a given user, including their booked services.</summary>
    /// <returns>The user's bookings, most recent start time first.</returns>
    Task<IReadOnlyList<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Checks whether a room already has a booking overlapping the given window.</summary>
    /// <param name="roomId">The room to check.</param>
    /// <param name="startTime">Start of the window to check.</param>
    /// <param name="endTime">End of the window to check.</param>
    Task<bool> ExistsOverlappingAsync(Guid roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

    /// <summary>Checks whether a room has any booking that hasn't ended yet.</summary>
    /// <param name="roomId">The room to check.</param>
    /// <param name="nowUtc">The current instant — a booking counts as active if its end time is after this.</param>
    Task<bool> HasActiveForRoomAsync(Guid roomId, DateTime nowUtc, CancellationToken cancellationToken);
}
