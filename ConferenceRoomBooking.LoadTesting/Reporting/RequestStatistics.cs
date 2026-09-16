namespace ConferenceRoomBooking.LoadTesting.Reporting;

public sealed record RequestStatistics(
    int TotalRequests,
    TimeSpan TotalTime,
    int SuccessCount,
    int FailureCount,
    double AverageMs,
    double MinMs,
    double MaxMs,
    double AverageConcurrency)
{
    public static RequestStatistics Compute(IReadOnlyCollection<RequestResult> results, TimeSpan totalTime)
    {
        var elapsedMs = results.Select(r => r.ElapsedMs).ToArray();
        var successCount = results.Count(r => r.Success);

        var averageConcurrency = elapsedMs.Sum() / totalTime.TotalMilliseconds;

        return new RequestStatistics(
            TotalRequests: results.Count,
            TotalTime: totalTime,
            SuccessCount: successCount,
            FailureCount: results.Count - successCount,
            AverageMs: elapsedMs.Average(),
            MinMs: elapsedMs.Min(),
            MaxMs: elapsedMs.Max(),
            AverageConcurrency: averageConcurrency);
    }
}
