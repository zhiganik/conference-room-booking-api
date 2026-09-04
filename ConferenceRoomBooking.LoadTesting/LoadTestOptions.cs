namespace ConferenceRoomBooking.LoadTesting;

public sealed record LoadTestOptions(int Requests, IReadOnlyList<int> ParallelismLevels, string BaseUrl)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var requests = 1000;
        var parallelismLevels = new List<int> { 10, 50, 100 };
        var baseUrl = "http://localhost:5000";

        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--requests" && int.TryParse(args[i + 1], out var parsedRequests))
            {
                requests = parsedRequests;
            }
            else if (args[i] == "--parallelism-levels")
            {
                parallelismLevels = args[i + 1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }
            else if (args[i] == "--base-url")
            {
                baseUrl = args[i + 1];
            }
        }

        return new LoadTestOptions(requests, parallelismLevels, baseUrl);
    }
}
