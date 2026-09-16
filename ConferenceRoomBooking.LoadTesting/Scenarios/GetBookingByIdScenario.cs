using ConferenceRoomBooking.LoadTesting.Auth;
using ConferenceRoomBooking.LoadTesting.Fixtures;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

/// <summary>
/// GetById requires an existing booking id. Until CreateBookingScenario has produced at least
/// one during this run, falls back to a harmless admin-readable endpoint instead of skipping.
/// </summary>
public sealed class GetBookingByIdScenario(TestDataFixture fixture) : RequestScenarioBase(Actor.Admin)
{
    public override string Name => "GET /api/bookings/{bookingId}";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex)
    {
        return fixture.TryGetRandomBookingId(Random.Shared, out var bookingId)
            ? SendAsync(httpClient, authContext, Actor, HttpMethod.Get, $"api/bookings/{bookingId}")
            : SendAsync(httpClient, authContext, Actor, HttpMethod.Get, "api/analytics/room-performance");
    }
}
