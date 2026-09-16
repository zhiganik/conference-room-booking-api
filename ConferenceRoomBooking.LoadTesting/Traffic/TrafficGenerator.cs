using System.Diagnostics;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Reporting;
using ConferenceRoomBooking.LoadTesting.Scenarios;

namespace ConferenceRoomBooking.LoadTesting.Traffic;

/// <summary>
/// Dispatches a total budget of requests, each a random pick from a <see cref="TrafficMix"/>,
/// capped at a maximum number running concurrently and spread out over time by an
/// <see cref="IPacingStrategy"/> — instead of firing everything in one burst.
/// </summary>
public sealed class TrafficGenerator(HttpClient httpClient, AuthContext authContext, IPacingStrategy pacing, int maxConcurrency, int? seed = null)
{
    // A seeded Random is not thread-safe (unlike Random.Shared), so scenario selection is
    // serialized behind a lock; contention is negligible next to the HTTP call that follows.
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
    private readonly Lock _randomLock = new();

    public async Task<RunReport> RunAsync(TrafficMix trafficMix, int totalRequests)
    {
        var results = new RequestResult[totalRequests];
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        var dispatched = 0;

        async Task DispatchOneAsync(int index)
        {
            await semaphore.WaitAsync();
            try
            {
                IRequestScenario scenario;
                lock (_randomLock)
                {
                    scenario = trafficMix.PickScenario(_random);
                }

                results[index] = await ExecuteAndTimeAsync(scenario, index);
            }
            finally
            {
                semaphore.Release();
            }
        }

        var overallStopwatch = Stopwatch.StartNew();
        var tasks = new List<Task>(totalRequests);
        for (var i = 0; i < totalRequests; i++)
        {
            await pacing.BeforeDispatchAsync(dispatched, CancellationToken.None);
            dispatched++;
            tasks.Add(DispatchOneAsync(i));
        }

        await Task.WhenAll(tasks);
        overallStopwatch.Stop();

        return RunReport.Build(results, overallStopwatch.Elapsed);
    }

    private async Task<RequestResult> ExecuteAndTimeAsync(IRequestScenario scenario, int index)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await scenario.ExecuteAsync(httpClient, authContext, index);
            stopwatch.Stop();
            return new RequestResult(scenario.Name, response.IsSuccessStatusCode, (int)response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds, Thread.CurrentThread.ManagedThreadId, Error: null);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new RequestResult(scenario.Name, false, StatusCode: 0,
                stopwatch.Elapsed.TotalMilliseconds, Thread.CurrentThread.ManagedThreadId, ex.Message);
        }
    }
}
