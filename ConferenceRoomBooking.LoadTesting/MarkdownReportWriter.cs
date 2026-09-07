using System.Text;

namespace ConferenceRoomBooking.LoadTesting;

public sealed class MarkdownReportWriter
{
    public void Write(string filePath, LoadTestOptions options, IReadOnlyList<ComparisonGroup> groups)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Load Test Results");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Target: {options.BaseUrl}");
        sb.AppendLine($"Requests per run: {options.Requests}");
        sb.AppendLine($"Parallelism levels: {string.Join(", ", options.ParallelismLevels)}");
        sb.AppendLine();

        sb.AppendLine("## Как запускать");
        sb.AppendLine();
        sb.AppendLine("1. Поднять API и БД из корня решения: `docker compose up -d`.");
        sb.AppendLine($"2. Дождаться, пока API станет доступен ({options.BaseUrl}).");
        sb.AppendLine("3. Запустить нагрузочный тест:");
        sb.AppendLine();
        sb.AppendLine("   ```");
        sb.AppendLine($"   dotnet run --project ConferenceRoomBooking.LoadTesting -- --requests {options.Requests} --parallelism-levels {string.Join(",", options.ParallelismLevels)} --base-url {options.BaseUrl}");
        sb.AppendLine("   ```");
        sb.AppendLine();
        sb.AppendLine("4. Результат — таблицы ниже, этот файл перезаписывается при каждом запуске.");
        sb.AppendLine();

        foreach (var group in groups)
        {
            sb.AppendLine($"## {group.Name}");
            sb.AppendLine();
            sb.AppendLine("| Parallelism | Total time (s) | Avg (ms) | Min (ms) | Max (ms) | Avg concurrency | Success | Failed |");
            sb.AppendLine("|---|---|---|---|---|---|---|---|");

            foreach (var (parallelism, stats) in group.Results)
            {
                sb.AppendLine(
                    $"| {parallelism} | {stats.TotalTime.TotalSeconds:F2} | {stats.AverageMs:F1} | {stats.MinMs:F1} | {stats.MaxMs:F1} | {stats.AverageConcurrency:F1} | {stats.SuccessCount} | {stats.FailureCount} |");
            }

            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString());
    }
}
