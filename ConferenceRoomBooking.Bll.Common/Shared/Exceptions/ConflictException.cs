namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// The request conflicts with the current state of the resource — e.g. a duplicate name where
/// uniqueness is required, or an attempt to delete something still referenced elsewhere. Mapped to
/// HTTP 409 Conflict.
/// </summary>
/// <param name="message">Client-safe explanation of the conflict.</param>
public class ConflictException(string message) : AppException(message);
