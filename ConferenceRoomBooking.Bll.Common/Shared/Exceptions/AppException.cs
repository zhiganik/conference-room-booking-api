namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// Base type for exceptions that represent an expected business-rule failure rather than a bug.
/// The Web layer's global exception handler maps every subclass to a specific HTTP status and
/// surfaces <see cref="Exception.Message"/> to the client; any other exception type is treated as
/// unexpected and returns a generic 500 with no message detail.
/// </summary>
/// <param name="message">Client-safe explanation of what went wrong.</param>
public abstract class AppException(string message) : Exception(message);
