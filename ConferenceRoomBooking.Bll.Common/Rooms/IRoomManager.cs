using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Rooms;

/// <summary>
/// Business rules for rooms: validating the offered services exist, soft-deleting instead of
/// hard-deleting, and refusing to delete a room with active or upcoming bookings.
/// </summary>
public interface IRoomManager
{
    /// <summary>Creates a new room.</summary>
    /// <param name="serviceOptionIds">Ids of the service options this room offers, or <see langword="null"/> for none.</param>
    /// <exception cref="NotFoundException">One or more of <paramref name="serviceOptionIds"/> doesn't exist.</exception>
    Task<Room> CreateAsync(string name, int capacity, decimal baseHourRate, List<Guid>? serviceOptionIds, CancellationToken cancellationToken);

    /// <summary>Retrieves a room by id.</summary>
    /// <exception cref="NotFoundException">No active room exists with the given id.</exception>
    Task<Room> GetByIdAsync(Guid roomId, CancellationToken cancellationToken);

    /// <summary>Updates a room's rate, capacity, and offered services.</summary>
    /// <param name="serviceOptionIds">Ids of the service options this room offers, replacing the previous set.</param>
    /// <exception cref="NotFoundException">No active room exists with the given id, or one of
    /// <paramref name="serviceOptionIds"/> doesn't exist.</exception>
    Task<Room> UpdateAsync(Guid roomId, string name, int capacity, decimal baseHourRate, List<Guid>? serviceOptionIds, CancellationToken cancellationToken);

    /// <summary>Soft-deletes a room.</summary>
    /// <exception cref="NotFoundException">No active room exists with the given id.</exception>
    /// <exception cref="ConflictException">The room has active or upcoming bookings.</exception>
    Task DeleteAsync(Guid roomId, CancellationToken cancellationToken);

    /// <summary>Finds rooms with at least the given capacity that are free for the whole given window.</summary>
    Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(DateTime startDate, DateTime endDate, int capacity, CancellationToken cancellationToken);
}
