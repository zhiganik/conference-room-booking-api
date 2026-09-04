namespace ConferenceRoomBooking.Bll.Bookings;

/// <summary>
/// Per-room mutex used to prevent double-booking a room when two requests race the availability
/// check and the insert. Implementations are expected to be registered as a singleton so the same
/// lock is shared across all scoped manager instances.
/// </summary>
public interface IRoomBookingLock
{
    /// <summary>
    /// Waits for and acquires the lock for the given room.
    /// </summary>
    /// <param name="roomId">The room to lock.</param>
    /// <returns>A token that releases the lock when disposed.</returns>
    Task<IDisposable> AcquireAsync(Guid roomId, CancellationToken cancellationToken);
}
