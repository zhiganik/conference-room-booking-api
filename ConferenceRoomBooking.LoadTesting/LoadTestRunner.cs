using System.Diagnostics;
using ConferenceRoomBooking.LoadTesting.Scenarios;

namespace ConferenceRoomBooking.LoadTesting;

/// <summary>
/// Runs a scenario <c>Requests</c> times with at most <c>Parallelism</c> requests in flight
/// at once. Knows nothing about GET/POST/PUT or endpoints — that's all in the scenario.
/// </summary>
public static class LoadTestRunner
{
    public static async Task<(RequestResult[] Results, TimeSpan TotalTime)> RunAsync(
        HttpClient httpClient, IRequestScenario scenario, LoadTestOptions options)
    {
        var results = new RequestResult[options.Requests];

        using var semaphore = new SemaphoreSlim(options.Parallelism);

        async Task RunOneAsync(int index)
        {
            await semaphore.WaitAsync();
            try
            {
                results[index] = await ExecuteAndTimeAsync(httpClient, scenario, index);
            }
            finally
            {
                semaphore.Release();
            }
        }

        var overallStopwatch = Stopwatch.StartNew();
        var tasks = Enumerable.Range(0, options.Requests).Select(RunOneAsync);
        await Task.WhenAll(tasks);
        overallStopwatch.Stop();

        return (results, overallStopwatch.Elapsed);
    }

    private static async Task<RequestResult> ExecuteAndTimeAsync(HttpClient httpClient, IRequestScenario scenario, int index)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await scenario.ExecuteAsync(httpClient, index);
            stopwatch.Stop();
            return new RequestResult(response.IsSuccessStatusCode, (int)response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds, Thread.CurrentThread.ManagedThreadId, Error: null);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new RequestResult(false, StatusCode: 0,
                stopwatch.Elapsed.TotalMilliseconds, Thread.CurrentThread.ManagedThreadId, ex.Message);
        }
    }
}
