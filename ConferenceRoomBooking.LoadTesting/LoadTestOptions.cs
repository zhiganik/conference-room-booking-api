namespace ConferenceRoomBooking.LoadTesting;

public sealed record LoadTestOptions(int TotalRequests, int MaxConcurrency, double RequestsPerSecond, string BaseUrl, int? Seed)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var totalRequests = 1000;
        var maxConcurrency = 10;
        var requestsPerSecond = 5.0;
        var baseUrl = "http://localhost:5000";
        int? seed = null;

        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--requests" && int.TryParse(args[i + 1], out var parsedRequests))
            {
                totalRequests = parsedRequests;
            }
            else if (args[i] == "--max-concurrency" && int.TryParse(args[i + 1], out var parsedConcurrency))
            {
                maxConcurrency = parsedConcurrency;
            }
            else if (args[i] == "--rate" && double.TryParse(args[i + 1], out var parsedRate))
            {
                requestsPerSecond = parsedRate;
            }
            else if (args[i] == "--base-url")
            {
                baseUrl = args[i + 1];
            }
            else if (args[i] == "--seed" && int.TryParse(args[i + 1], out var parsedSeed))
            {
                seed = parsedSeed;
            }
        }

        return new LoadTestOptions(totalRequests, maxConcurrency, requestsPerSecond, baseUrl, seed);
    }
}
