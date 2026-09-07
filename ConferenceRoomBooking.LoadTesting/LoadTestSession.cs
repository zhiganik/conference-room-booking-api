using ConferenceRoomBooking.LoadTesting.Scenarios;

namespace ConferenceRoomBooking.LoadTesting;

public sealed class LoadTestSession(HttpClient httpClient, LoadTestOptions options)
{
    public async Task<RequestStatistics> RunAndReportAsync(IRequestScenario scenario, int parallelism)
    {
        Console.WriteLine();
        Console.WriteLine($"Running '{scenario.Name}': {options.Requests} requests, parallelism {parallelism}, target {options.BaseUrl}");
        var (results, totalTime) = await LoadTestRunner.RunAsync(httpClient, scenario, options.Requests, parallelism);
        var stats = RequestStatistics.Compute(results, totalTime, parallelism);
        ConsoleReporter.Report(scenario.Name, stats, results);
        return stats;
    }
}
