namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// The caller isn't authenticated, or presented invalid credentials. Mapped to HTTP 401 Unauthorized.
/// </summary>
/// <param name="template">Client-safe message template.</param>
/// <param name="args">Values substituted into <paramref name="template"/>'s placeholders, in order.</param>
public class UnauthorizedException(string template, params object?[] args) : AppException(template, args);
