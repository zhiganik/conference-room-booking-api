using ConferenceRoomBooking.LoadTesting.Fixtures;
using ConferenceRoomBooking.LoadTesting.Scenarios;

namespace ConferenceRoomBooking.LoadTesting.Traffic;

/// <summary>
/// The declarative registry of every scenario in the random mix and its relative weight. Add or
/// remove an endpoint from the run by editing this one list.
/// </summary>
public sealed class TrafficMix
{
    private readonly WeightedRandomPicker<IRequestScenario> _picker;

    private TrafficMix(WeightedRandomPicker<IRequestScenario> picker)
    {
        _picker = picker;
    }

    public IRequestScenario PickScenario(Random random) => _picker.Pick(random);

    public static TrafficMix BuildDefault(TestDataFixture fixture, RunMarker marker, string seedUserEmail, string seedUserPassword)
    {
        var entries = new List<(IRequestScenario Scenario, double Weight)>
        {
            // Read-only — the bulk of realistic traffic.
            (new GetAvailableRoomsScenario(), 12),
            (new GetRoomByIdScenario(fixture), 8),
            (new SearchServiceOptionsScenario(), 6),
            (new GetServiceOptionByIdScenario(fixture), 5),
            (new GetMyBookingsScenario(), 6),
            (new GetBookingByIdScenario(fixture), 5),
            (new GetRoomPerformanceScenario(), 3),
            (new GetServicePerformanceScenario(), 3),
            (new LoginScenario(seedUserEmail, seedUserPassword), 4),

            // Mutating, fixture-scoped — always targets synthetic rooms/service options only.
            (new UpdateRoomScenario(fixture), 4),
            (new CreateBookingScenario(fixture), 10),
            (new UpdateServiceOptionScenario(fixture), 3),

            // Mutating, self-contained — each dispatch creates and disposes of its own tagged entity.
            (new CreateRoomScenario(marker), 3),
            (new DeleteRoomScenario(marker), 3),
            (new CreateServiceOptionScenario(marker), 3),
            (new DeleteServiceOptionScenario(marker), 3),
            (new RegisterUserScenario(marker, fixture), 3)
        };

        return new TrafficMix(new WeightedRandomPicker<IRequestScenario>(entries));
    }
}
