namespace ConferenceRoomBooking.LoadTesting;

public sealed record LoadTestOptions(int Requests, int Parallelism, string BaseUrl)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var requests = 1000;
        var parallelism = 10;
        var baseUrl = "http://localhost:5000";

        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--requests" && int.TryParse(args[i + 1], out var parsedRequests))
            {
                requests = parsedRequests;
            }
            else if (args[i] == "--parallelism" && int.TryParse(args[i + 1], out var parsedParallelism))
            {
                parallelism = parsedParallelism;
            }
            else if (args[i] == "--base-url")
            {
                baseUrl = args[i + 1];
            }
        }

        return new LoadTestOptions(requests, parallelism, baseUrl);
    }
}
