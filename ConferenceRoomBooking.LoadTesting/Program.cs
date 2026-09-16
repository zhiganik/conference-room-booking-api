using ConferenceRoomBooking.LoadTesting;
using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;
using ConferenceRoomBooking.LoadTesting.Reporting;
using ConferenceRoomBooking.LoadTesting.Traffic;

const string seedUserEmail = "seed.bookings@conference-room-booking.local";
const string seedUserPassword = "Seed@Bookings123!";
const string adminEmail = "admin@gmail.com";
const string adminPassword = "Admin1234!";

var options = LoadTestOptions.Parse(args);

using var httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };

var authentication = new Authentication(httpClient);
var userToken = await authentication.LoginAsync(seedUserEmail, seedUserPassword);
var adminToken = await authentication.LoginAsync(adminEmail, adminPassword);
var authContext = new AuthContext(userToken, adminToken);
Console.WriteLine("Logged in as seed user and admin");

var marker = RunMarker.Create();
Console.WriteLine($"Run marker: {marker.Value}");

var fixture = await new TestDataFixtureBuilder(httpClient, authContext, marker).BuildAsync();
Console.WriteLine($"Created {fixture.Rooms.Count} fixture rooms and {fixture.ServiceOptionIds.Count} fixture service options");

var trafficMix = TrafficMix.BuildDefault(fixture, marker, seedUserEmail, seedUserPassword);
var pacing = new RateLimitedPacing(options.RequestsPerSecond);
var generator = new TrafficGenerator(httpClient, authContext, pacing, options.MaxConcurrency, options.Seed);

Console.WriteLine();
Console.WriteLine(
    $"Dispatching {options.TotalRequests} requests, max concurrency {options.MaxConcurrency}, " +
    $"~{options.RequestsPerSecond:F1} req/s, target {options.BaseUrl}");
var report = await generator.RunAsync(trafficMix, options.TotalRequests);

ConsoleReporter.ReportRun(report, options.MaxConcurrency);
ConsoleReporter.ReportCleanup(marker, fixture);

var reportPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "load-test-report.md"));
new MarkdownReportWriter().Write(reportPath, options, report, marker, fixture);
Console.WriteLine();
Console.WriteLine($"Markdown report written to {reportPath}");
