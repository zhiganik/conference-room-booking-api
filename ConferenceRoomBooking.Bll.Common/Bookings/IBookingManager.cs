using ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Bookings;

/// <summary>
/// Business rules for creating and retrieving bookings: pricing, validating the requested services
/// are actually offered by the room, and preventing double-booking under concurrent requests.
/// </summary>
public interface IBookingManager
{
    /// <summary>
    /// Books a room for the current user (resolved from <see cref="IUserContext"/>).
    /// </summary>
    /// <param name="roomId">The room to book.</param>
    /// <param name="startTime">The booking's start time.</param>
    /// <param name="durationMinutes">The booking's duration, in minutes.</param>
    /// <param name="serviceOptionIds">Ids of the service options to add to the booking, or <see langword="null"/> for none.</param>
    /// <exception cref="UnauthorizedException">The caller isn't authenticated.</exception>
    /// <exception cref="NotFoundException">The room doesn't exist, or one of <paramref name="serviceOptionIds"/> doesn't exist.</exception>
    /// <exception cref="ConflictException">One or more of <paramref name="serviceOptionIds"/> isn't offered by this room.</exception>
    /// <exception cref="RoomUnavailableException">The room is already booked for some or all of the requested window.</exception>
    Task<Booking> CreateAsync(Guid roomId, DateTime startTime, int durationMinutes, List<Guid>? serviceOptionIds, CancellationToken cancellationToken);

    /// <summary>Retrieves a booking by id.</summary>
    /// <exception cref="NotFoundException">No booking exists with the given id.</exception>
    Task<Booking> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken);

    /// <summary>Retrieves every booking made by the current user.</summary>
    /// <exception cref="UnauthorizedException">The caller isn't authenticated.</exception>
    Task<IReadOnlyList<Booking>> GetByUserAsync(CancellationToken cancellationToken);
}
