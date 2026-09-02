using ConferenceRoomBooking.Bll.Common.Rooms.Models;

namespace ConferenceRoomBooking.Bll.Common.Rooms;

/// <summary>
/// Persistence for <c>Rooms</c> and their offered services (<c>RoomServiceOptions</c>). Rooms are
/// soft-deleted — every read here excludes rows with <c>IsDeleted = 1</c>.
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Inserts a new room together with its offered services, in one transaction.
    /// </summary>
    /// <param name="room">The room to create. <see cref="Room.Services"/> determines the
    /// <c>RoomServiceOptions</c> rows written alongside it.</param>
    /// <returns>The created room, including its generated <see cref="Room.Id"/> and services.</returns>
    Task<Room> CreateAsync(Room room, CancellationToken cancellationToken);

    /// <summary>Looks up a non-deleted room by id, including its offered services.</summary>
    /// <returns>The matching room, or <see langword="null"/> if no such active room exists.</returns>
    Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken);

    /// <summary>Looks up a non-deleted room by name, including its offered services.</summary>
    /// <returns>The matching room, or <see langword="null"/> if no active room has that name.</returns>
    Task<Room?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing room's fields and replaces its offered services with
    /// <see cref="Room.Services"/>, in one transaction.
    /// </summary>
    Task UpdateAsync(Room room, CancellationToken cancellationToken);

    /// <summary>Flags a room as deleted (<c>IsDeleted = 1</c>) without removing its row or booking history.</summary>
    Task SoftDeleteAsync(int roomId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds non-deleted rooms with at least the given capacity and no booking overlapping the
    /// given window.
    /// </summary>
    /// <param name="capacity">Minimum required capacity.</param>
    /// <param name="startTime">Start of the requested window.</param>
    /// <param name="endTime">End of the requested window.</param>
    /// <returns>Matching rooms, ordered by name.</returns>
    Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(int capacity, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}
