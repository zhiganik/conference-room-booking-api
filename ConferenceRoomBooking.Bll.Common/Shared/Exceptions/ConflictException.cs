namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// The request conflicts with the current state of the resource — e.g. a duplicate name where
/// uniqueness is required, or an attempt to delete something still referenced elsewhere. Mapped to
/// HTTP 409 Conflict.
/// </summary>
/// <param name="template">Client-safe message template, e.g. "Room '{RoomName}' is already booked."</param>
/// <param name="args">Values substituted into <paramref name="template"/>'s placeholders, in order.</param>
public class ConflictException(string template, params object?[] args) : AppException(template, args);
