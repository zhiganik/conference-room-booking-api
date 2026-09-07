namespace ConferenceRoomBooking.LoadTesting;

public static class ConsoleReporter
{
    public static void Report(string scenarioName, RequestStatistics stats, RequestResult[] results)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {scenarioName} ===");
        Console.WriteLine($"Total requests:       {stats.TotalRequests}");
        Console.WriteLine($"Concurrency (limit):  {stats.Parallelism}");
        Console.WriteLine($"Avg concurrency:      {stats.AverageConcurrency:F1}");
        Console.WriteLine($"Total time:           {stats.TotalTime.TotalSeconds:F2}s");
        Console.WriteLine($"Successful requests:  {stats.SuccessCount}");
        Console.WriteLine($"Failed requests:      {stats.FailureCount}");
        Console.WriteLine($"Avg response time:    {stats.AverageMs:F1}ms");
        Console.WriteLine($"Min response time:    {stats.MinMs:F1}ms");
        Console.WriteLine($"Max response time:    {stats.MaxMs:F1}ms");

        var threadGroups = results
            .GroupBy(r => r.ThreadId)
            .OrderByDescending(group => group.Count())
            .ToList();

        Console.WriteLine();
        Console.WriteLine($"=== Threads (used {threadGroups.Count}) ===");
        foreach (var group in threadGroups)
        {
            Console.WriteLine($"Thread {group.Key,-6} handled {group.Count()} request(s)");
        }
    }
}
