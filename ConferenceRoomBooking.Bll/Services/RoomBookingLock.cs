using System.Collections.Concurrent;

namespace ConferenceRoomBooking.Bll.Services;

/// <summary>
/// Singleton, per-room mutex used to prevent double-booking a room when two requests
/// race the availability check + insert. Register as a singleton so the semaphores
/// are shared across all scoped manager instances.
/// </summary>
public class RoomBookingLock : IRoomBookingLock
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _locksByRoomId = new();

    public async Task<IDisposable> AcquireAsync(int roomId, CancellationToken cancellationToken)
    {
        var semaphore = _locksByRoomId.GetOrAdd(roomId, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        return semaphore;
    }
}
