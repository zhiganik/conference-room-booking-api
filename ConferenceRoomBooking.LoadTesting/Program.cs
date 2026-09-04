using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ConferenceRoomBooking.LoadTesting;
using ConferenceRoomBooking.LoadTesting.Scenarios;

var options = LoadTestOptions.Parse(args);

using var httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };

var loginPayload = JsonSerializer.Serialize(new
{
    Email = "seed.bookings@conference-room-booking.local",
    Password = "Seed@Bookings123!"
});

using var loginResponse = await httpClient.PostAsync("api/auth/login", new StringContent(loginPayload, Encoding.UTF8, "application/json"));
loginResponse.EnsureSuccessStatusCode();

var loginBody = await loginResponse.Content.ReadAsStringAsync();
var accessToken = JsonDocument.Parse(loginBody).RootElement.GetProperty("accessToken").GetString();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

IRequestScenario scenario = new GetAvailableRoomsScenario();

Console.WriteLine($"Running '{scenario.Name}': {options.Requests} requests, parallelism {options.Parallelism}, target {options.BaseUrl}");

var (results, totalTime) = await LoadTestRunner.RunAsync(httpClient, scenario, options);
var stats = RequestStatistics.Compute(results, totalTime, options.Parallelism);

ConsoleReporter.Report(scenario.Name, stats, results);
