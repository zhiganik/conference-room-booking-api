using System.Collections.Concurrent;
using ConferenceRoomBooking.LoadTesting.Dtos;

namespace ConferenceRoomBooking.LoadTesting.Fixtures;

/// <summary>
/// The pool of disposable, tagged entities scenarios are allowed to read/mutate during a run.
/// Rooms and service options are seeded once before the run starts (single-threaded) and only
/// read afterward; booking ids accumulate concurrently as CreateBookingScenario succeeds.
/// </summary>
public sealed class TestDataFixture(RunMarker marker)
{
    private readonly List<AvailableRoomDto> _rooms = [];
    private readonly List<Guid> _serviceOptionIds = [];
    private readonly ConcurrentBag<Guid> _createdBookingIds = [];
    private readonly ConcurrentBag<string> _registeredUserEmails = [];
    private readonly ConcurrentDictionary<Guid, int> _roomSlotCounters = new();

    public RunMarker Marker => marker;

    public IReadOnlyList<AvailableRoomDto> Rooms => _rooms;

    public IReadOnlyList<Guid> ServiceOptionIds => _serviceOptionIds;

    public IReadOnlyCollection<string> RegisteredUserEmails => _registeredUserEmails;

    public IReadOnlyCollection<Guid> CreatedBookingIds => _createdBookingIds;

    internal void AddRoom(AvailableRoomDto room) => _rooms.Add(room);

    internal void AddServiceOption(Guid serviceOptionId) => _serviceOptionIds.Add(serviceOptionId);

    public AvailableRoomDto GetRandomRoom(Random random) => _rooms[random.Next(_rooms.Count)];

    public Guid GetRandomServiceOptionId(Random random) => _serviceOptionIds[random.Next(_serviceOptionIds.Count)];

    /// <summary>Returns a per-room, thread-safe incrementing slot index for booking scenarios so
    /// concurrently dispatched requests never collide on the same room/time slot.</summary>
    public int NextSlotOffset(Guid roomId) => _roomSlotCounters.AddOrUpdate(roomId, 1, (_, current) => current + 1);

    public void RegisterCreatedBooking(Guid bookingId) => _createdBookingIds.Add(bookingId);

    public void RegisterCreatedUser(string email) => _registeredUserEmails.Add(email);

    public bool TryGetRandomBookingId(Random random, out Guid bookingId)
    {
        var snapshot = _createdBookingIds.ToArray();
        if (snapshot.Length == 0)
        {
            bookingId = default;
            return false;
        }

        bookingId = snapshot[random.Next(snapshot.Length)];
        return true;
    }
}
