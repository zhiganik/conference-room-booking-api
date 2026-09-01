namespace ConferenceRoomBooking.Bll.Bookings;

public interface IRoomBookingLock
{
    Task<IDisposable> AcquireAsync(int roomId, CancellationToken cancellationToken);
}
