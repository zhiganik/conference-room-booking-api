using System.Collections.Concurrent;

namespace ConferenceRoomBooking.Bll.Bookings;

public class RoomBookingLock : IRoomBookingLock
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locksByRoomId = new();

    public async Task<IDisposable> AcquireAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var semaphore = _locksByRoomId.GetOrAdd(roomId, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        return semaphore;
    }
}
