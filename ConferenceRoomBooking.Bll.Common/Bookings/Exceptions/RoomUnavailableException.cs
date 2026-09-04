using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;

/// <summary>
/// The requested room is already booked for some or all of the requested time window. Distinct
/// from <see cref="ConflictException"/> so callers/logs can tell "double-booking" apart from other
/// conflicts. Mapped to HTTP 409 Conflict.
/// </summary>
/// <param name="message">Client-safe explanation of the unavailability.</param>
public class RoomUnavailableException(string message) : AppException(message);
