namespace ConferenceRoomBooking.Application.Services;

public interface IRoomBookingLock
{
    Task<IDisposable> AcquireAsync(int roomId, CancellationToken cancellationToken);
}
