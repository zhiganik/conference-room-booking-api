namespace ConferenceRoomBooking.LoadTesting.Reporting;

public sealed record ScenarioBreakdown(string ScenarioName, RequestStatistics Stats);

public sealed record RunReport(
    RequestStatistics Overall,
    IReadOnlyList<ScenarioBreakdown> ByScenario,
    IReadOnlyList<RequestResult> AllResults)
{
    public static RunReport Build(RequestResult[] results, TimeSpan totalTime)
    {
        var overall = RequestStatistics.Compute(results, totalTime);
        var byScenario = results
            .GroupBy(r => r.ScenarioName)
            .OrderByDescending(g => g.Count())
            .Select(g => new ScenarioBreakdown(g.Key, RequestStatistics.Compute(g.ToArray(), totalTime)))
            .ToList();

        return new RunReport(overall, byScenario, results);
    }
}
