namespace ConferenceRoomBooking.LoadTesting;

public sealed record RequestStatistics(
    int TotalRequests,
    int Parallelism,
    TimeSpan TotalTime,
    int SuccessCount,
    int FailureCount,
    double AverageMs,
    double MinMs,
    double MaxMs,
    double AverageConcurrency)
{
    public static RequestStatistics Compute(RequestResult[] results, TimeSpan totalTime, int parallelism)
    {
        var elapsedMs = results.Select(r => r.ElapsedMs).ToArray();
        var successCount = results.Count(r => r.Success);

        var averageConcurrency = elapsedMs.Sum() / totalTime.TotalMilliseconds;

        return new RequestStatistics(
            TotalRequests: results.Length,
            Parallelism: parallelism,
            TotalTime: totalTime,
            SuccessCount: successCount,
            FailureCount: results.Length - successCount,
            AverageMs: elapsedMs.Average(),
            MinMs: elapsedMs.Min(),
            MaxMs: elapsedMs.Max(),
            AverageConcurrency: averageConcurrency);
    }
}
