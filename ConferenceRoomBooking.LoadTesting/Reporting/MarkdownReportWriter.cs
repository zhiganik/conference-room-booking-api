using System.Text;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Reporting;

public sealed class MarkdownReportWriter
{
    public void Write(string filePath, LoadTestOptions options, RunReport report, RunMarker marker, TestDataFixture fixture)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Load Test Results");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Target: {options.BaseUrl}");
        sb.AppendLine($"Run marker: {marker.Value}");
        sb.AppendLine($"Total requests: {options.TotalRequests}");
        sb.AppendLine($"Max concurrency: {options.MaxConcurrency}");
        sb.AppendLine($"Target rate: {options.RequestsPerSecond:F1} req/s (jittered)");
        sb.AppendLine();

        sb.AppendLine("## Как запускать");
        sb.AppendLine();
        sb.AppendLine("1. Поднять API и БД из корня решения: `docker compose up -d`.");
        sb.AppendLine($"2. Дождаться, пока API станет доступен ({options.BaseUrl}).");
        sb.AppendLine("3. Запустить нагрузочный тест:");
        sb.AppendLine();
        sb.AppendLine("   ```");
        sb.AppendLine(
            $"   dotnet run --project ConferenceRoomBooking.LoadTesting -- --requests {options.TotalRequests} " +
            $"--max-concurrency {options.MaxConcurrency} --rate {options.RequestsPerSecond} --base-url {options.BaseUrl}");
        sb.AppendLine("   ```");
        sb.AppendLine();
        sb.AppendLine("4. Каждый запуск случайно перемешивает все доступные сценарии (GET/POST/PUT/DELETE) вместо");
        sb.AppendLine("   последовательных пакетов, и не более `--max-concurrency` запросов выполняется одновременно.");
        sb.AppendLine("5. Все созданные тестовые данные помечены префиксом run marker — см. раздел очистки ниже.");
        sb.AppendLine("   Этот файл перезаписывается при каждом запуске.");
        sb.AppendLine();

        sb.AppendLine("## Overall");
        sb.AppendLine();
        sb.AppendLine("| Total | Max concurrency | Avg concurrency | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Success | Failed |");
        sb.AppendLine("|---|---|---|---|---|---|---|---|---|");
        var overall = report.Overall;
        sb.AppendLine(
            $"| {overall.TotalRequests} | {options.MaxConcurrency} | {overall.AverageConcurrency:F1} | " +
            $"{overall.TotalTime.TotalSeconds:F2} | {overall.AverageMs:F1} | {overall.MinMs:F1} | {overall.MaxMs:F1} | " +
            $"{overall.SuccessCount} | {overall.FailureCount} |");
        sb.AppendLine();

        sb.AppendLine("## By scenario");
        sb.AppendLine();
        sb.AppendLine("| Scenario | Count | Success | Failed | Avg (ms) | Min (ms) | Max (ms) |");
        sb.AppendLine("|---|---|---|---|---|---|---|");
        foreach (var breakdown in report.ByScenario)
        {
            var stats = breakdown.Stats;
            sb.AppendLine(
                $"| {breakdown.ScenarioName} | {stats.TotalRequests} | {stats.SuccessCount} | {stats.FailureCount} | " +
                $"{stats.AverageMs:F1} | {stats.MinMs:F1} | {stats.MaxMs:F1} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Test data created (needs manual cleanup)");
        sb.AppendLine();
        sb.AppendLine($"Run marker: `{marker.Value}`. Everything this run created is tagged — find it all via " +
                       $"`Name LIKE '{marker.Value}%'`.");
        sb.AppendLine();
        sb.AppendLine("| Room id | Room name |");
        sb.AppendLine("|---|---|");
        foreach (var room in fixture.Rooms)
        {
            sb.AppendLine($"| {room.Id} | {room.Name} |");
        }
        sb.AppendLine();
        sb.AppendLine("| Service option id |");
        sb.AppendLine("|---|");
        foreach (var serviceOptionId in fixture.ServiceOptionIds)
        {
            sb.AppendLine($"| {serviceOptionId} |");
        }
        sb.AppendLine();
        if (fixture.RegisteredUserEmails.Count > 0)
        {
            sb.AppendLine("| Registered user email |");
            sb.AppendLine("|---|");
            foreach (var email in fixture.RegisteredUserEmails)
            {
                sb.AppendLine($"| {email} |");
            }
            sb.AppendLine();
        }
        sb.AppendLine($"Bookings created this run: {fixture.CreatedBookingIds.Count}. Bookings have no delete/cancel API");
        sb.AppendLine("endpoint — they are only reachable by joining `Bookings.RoomId` against the fixture room ids above,");
        sb.AppendLine("so a direct DB query is the only way to purge them.");
        sb.AppendLine();
        sb.AppendLine($"Note: `CreateRoomScenario`/`CreateServiceOptionScenario` dispatches also leave extra tagged, undeleted");
        sb.AppendLine($"entities named `{marker.Value}_Room_Extra*` / `{marker.Value}_Service_Extra*` not listed above — the");
        sb.AppendLine("same `Name LIKE` query covers them.");

        File.WriteAllText(filePath, sb.ToString());
    }
}
