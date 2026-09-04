using System.Net.Http.Headers;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting;
using ConferenceRoomBooking.LoadTesting.Scenarios;

var options = LoadTestOptions.Parse(args);
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

using var httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };

var authentication = new Authentication(httpClient);
var session = new LoadTestSession(httpClient, options);

var userToken = await authentication.LoginAsync("seed.bookings@conference-room-booking.local", "Seed@Bookings123!");
var adminToken = await authentication.LoginAsync("admin@gmail.com", "Admin1234!");
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
Console.WriteLine("Logged in as seed user and admin");

using var roomsResponse = await new GetAvailableRoomsScenario().ExecuteAsync(httpClient, 0);
roomsResponse.EnsureSuccessStatusCode();
var result = await roomsResponse.Content.ReadAsStringAsync();
var rooms = JsonSerializer.Deserialize<List<AvailableRoomDto>>(result, jsonOptions);

if (rooms is null || rooms.Count == 0)
{
    throw new InvalidOperationException("No available rooms returned by the API.");
}

var room = rooms[0];
Console.WriteLine($"Targeting room '{room.Name}' ({room.Id})");

var runSaltDays = Random.Shared.Next(0, 5000);
var groups = new List<ComparisonGroup>();

var getResults = new List<(int Parallelism, RequestStatistics Stats)>();
foreach (var parallelism in options.ParallelismLevels)
{
    var stats = await session.RunAndReportAsync(new GetAvailableRoomsScenario(), parallelism);
    getResults.Add((parallelism, stats));
}
groups.Add(new ComparisonGroup("GET /api/rooms/available", getResults));

var postResults = new List<(int Parallelism, RequestStatistics Stats)>();
for (var levelIndex = 0; levelIndex < options.ParallelismLevels.Count; levelIndex++)
{
    var parallelism = options.ParallelismLevels[levelIndex];
    var baseDate = DateTime.UtcNow.Date.AddDays(30 + runSaltDays + levelIndex * 70);
    var stats = await session.RunAndReportAsync(new CreateBookingScenario(room.Id, baseDate), parallelism);
    postResults.Add((parallelism, stats));
}
groups.Add(new ComparisonGroup("POST /api/bookings", postResults));

httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
var putResults = new List<(int Parallelism, RequestStatistics Stats)>();
foreach (var parallelism in options.ParallelismLevels)
{
    var scenario = new UpdateRoomScenario(room.Id, room.Name, room.Capacity, room.BaseHourlyRate);
    var stats = await session.RunAndReportAsync(scenario, parallelism);
    putResults.Add((parallelism, stats));
}
groups.Add(new ComparisonGroup("PUT /api/rooms/{roomId}", putResults));

var reportPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "load-test-report.md"));
new MarkdownReportWriter().Write(reportPath, options, groups);
Console.WriteLine();
Console.WriteLine($"Markdown report written to {reportPath}");
