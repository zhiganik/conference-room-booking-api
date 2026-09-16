using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;

/// <summary>
/// The requested room is already booked for some or all of the requested time window. Distinct
/// from <see cref="ConflictException"/> so callers/logs can tell "double-booking" apart from other
/// conflicts. Mapped to HTTP 409 Conflict.
/// </summary>
/// <param name="template">Client-safe message template.</param>
/// <param name="args">Values substituted into <paramref name="template"/>'s placeholders, in order.</param>
public class RoomUnavailableException(string template, params object?[] args) : AppException(template, args);
