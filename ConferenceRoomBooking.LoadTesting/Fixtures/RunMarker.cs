namespace ConferenceRoomBooking.LoadTesting.Fixtures;

/// <summary>
/// Identifies one load-test run so every entity it creates against a (possibly production)
/// target can be found and deleted manually afterward. Never reused across runs.
/// </summary>
public sealed record RunMarker(string Value)
{
    public static RunMarker Create() =>
        new($"LOADTEST_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..4]}");

    public string RoomName(string suffix) => $"{Value}_Room_{suffix}";

    public string ServiceOptionName(string suffix) => $"{Value}_Service_{suffix}";

    public string UserEmail(string suffix) =>
        $"loadtest.{Value.ToLowerInvariant()}.{suffix}@conference-room-booking.local";
}
