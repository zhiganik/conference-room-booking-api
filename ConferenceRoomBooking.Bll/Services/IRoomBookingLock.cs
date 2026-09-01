namespace ConferenceRoomBooking.Bll.Services;

public interface IRoomBookingLock
{
    Task<IDisposable> AcquireAsync(int roomId, CancellationToken cancellationToken);
}
