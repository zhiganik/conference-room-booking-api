using ConferenceRoomBooking.LoadTesting.Auth;

namespace ConferenceRoomBooking.LoadTesting.Scenarios;

public sealed class GetMyBookingsScenario() : RequestScenarioBase(Actor.User)
{
    public override string Name => "GET /api/bookings/my";

    public override Task<HttpResponseMessage> ExecuteAsync(HttpClient httpClient, AuthContext authContext, int requestIndex) =>
        SendAsync(httpClient, authContext, Actor, HttpMethod.Get, "api/bookings/my");
}
