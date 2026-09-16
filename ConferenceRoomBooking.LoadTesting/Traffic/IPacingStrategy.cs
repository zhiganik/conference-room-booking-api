namespace ConferenceRoomBooking.LoadTesting.Traffic;

/// <summary>Controls when the next request is dispatched, independent of the concurrency cap.</summary>
public interface IPacingStrategy
{
    Task BeforeDispatchAsync(int dispatchedSoFar, CancellationToken cancellationToken);
}
