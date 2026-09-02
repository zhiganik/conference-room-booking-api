namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// The caller isn't authenticated, or presented invalid credentials. Mapped to HTTP 401 Unauthorized.
/// </summary>
/// <param name="message">Client-safe explanation of the authentication failure.</param>
public class UnauthorizedException(string message) : AppException(message);
