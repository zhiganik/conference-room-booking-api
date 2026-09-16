using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Reporting;

public static class ConsoleReporter
{
    public static void ReportRun(RunReport report, int maxConcurrency)
    {
        Console.WriteLine();
        Console.WriteLine("=== Overall ===");
        Console.WriteLine($"Total requests:       {report.Overall.TotalRequests}");
        Console.WriteLine($"Max concurrency:      {maxConcurrency}");
        Console.WriteLine($"Avg concurrency:      {report.Overall.AverageConcurrency:F1}");
        Console.WriteLine($"Total time:           {report.Overall.TotalTime.TotalSeconds:F2}s");
        Console.WriteLine($"Successful requests:  {report.Overall.SuccessCount}");
        Console.WriteLine($"Failed requests:      {report.Overall.FailureCount}");
        Console.WriteLine($"Avg response time:    {report.Overall.AverageMs:F1}ms");
        Console.WriteLine($"Min response time:    {report.Overall.MinMs:F1}ms");
        Console.WriteLine($"Max response time:    {report.Overall.MaxMs:F1}ms");

        Console.WriteLine();
        Console.WriteLine("=== By scenario ===");
        foreach (var breakdown in report.ByScenario)
        {
            var stats = breakdown.Stats;
            Console.WriteLine(
                $"{breakdown.ScenarioName,-40} count={stats.TotalRequests,-5} success={stats.SuccessCount,-5} " +
                $"failed={stats.FailureCount,-5} avg={stats.AverageMs,7:F1}ms min={stats.MinMs,7:F1}ms max={stats.MaxMs,7:F1}ms");
        }

        var threadGroups = report.AllResults
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

    public static void ReportCleanup(RunMarker marker, TestDataFixture fixture)
    {
        Console.WriteLine();
        Console.WriteLine("=== Test data created (needs manual cleanup) ===");
        Console.WriteLine($"Run marker: {marker.Value}");
        Console.WriteLine($"Everything this run created is tagged — find it all via: Name LIKE '{marker.Value}%'");

        Console.WriteLine();
        Console.WriteLine("Fixture rooms:");
        foreach (var room in fixture.Rooms)
        {
            Console.WriteLine($"  {room.Id}  {room.Name}");
        }

        Console.WriteLine("Fixture service options:");
        foreach (var serviceOptionId in fixture.ServiceOptionIds)
        {
            Console.WriteLine($"  {serviceOptionId}");
        }

        if (fixture.RegisteredUserEmails.Count > 0)
        {
            Console.WriteLine("Registered users:");
            foreach (var email in fixture.RegisteredUserEmails)
            {
                Console.WriteLine($"  {email}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Bookings created this run: {fixture.CreatedBookingIds.Count}");
        Console.WriteLine("Bookings have no delete/cancel API endpoint. They are only reachable by joining");
        Console.WriteLine("Bookings.RoomId against the fixture room ids above — purge them via a direct DB query if needed.");
        Console.WriteLine("(CreateRoomScenario/CreateServiceOptionScenario dispatches also leave extra tagged, undeleted");
        Console.WriteLine($"entities named '{marker.Value}_Room_Extra*' / '{marker.Value}_Service_Extra*' — same cleanup query covers them.)");
    }
}
