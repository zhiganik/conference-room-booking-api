using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Bll.Bookings;

public class RoomBookingLock(ILogger<RoomBookingLock> logger) : IRoomBookingLock
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locksByRoomId = new();

    public async Task<IDisposable> AcquireAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var semaphore = _locksByRoomId.GetOrAdd(roomId, static _ => new SemaphoreSlim(1, 1));

        var isContended = semaphore.CurrentCount == 0;
        if (isContended)
        {
            logger.LogDebug("Waiting for booking lock on room {RoomId}: another request holds it.", roomId);
        }

        await semaphore.WaitAsync(cancellationToken);
        logger.LogDebug("Acquired booking lock on room {RoomId}.", roomId);

        return new Releaser(semaphore, roomId, logger);
    }

    private sealed record Releaser(SemaphoreSlim Semaphore, Guid RoomId, ILogger Logger) : IDisposable
    {
        public void Dispose()
        {
            Semaphore.Release();
            Logger.LogDebug("Released booking lock on room {RoomId}.", RoomId);
        }
    }
}
