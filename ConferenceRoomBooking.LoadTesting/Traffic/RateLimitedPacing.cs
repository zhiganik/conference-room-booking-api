namespace ConferenceRoomBooking.LoadTesting.Traffic;

/// <summary>
/// Spreads dispatches out over wall-clock time at an average target rate, with random jitter,
/// instead of firing everything at once. The concurrency cap (a separate semaphore in
/// <see cref="TrafficGenerator"/>) still bounds how many run at the same time.
/// </summary>
public sealed class RateLimitedPacing(double requestsPerSecond, double jitterFraction = 0.4) : IPacingStrategy
{
    private readonly double _averageDelayMs = 1000.0 / requestsPerSecond;

    public async Task BeforeDispatchAsync(int dispatchedSoFar, CancellationToken cancellationToken)
    {
        if (dispatchedSoFar == 0)
        {
            return;
        }

        var jitterMultiplier = 1.0 + ((Random.Shared.NextDouble() * 2.0 - 1.0) * jitterFraction);
        var delayMs = Math.Max(0, _averageDelayMs * jitterMultiplier);
        if (delayMs > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
        }
    }
}
