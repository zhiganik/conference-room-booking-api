namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// No entity of the given type exists for the given key. Mapped to HTTP 404 Not Found.
/// </summary>
/// <param name="entityName">Name of the entity type that couldn't be found, e.g. <c>nameof(Room)</c>.</param>
/// <param name="key">The key that was looked up (formatted into the message via <see cref="object.ToString"/>).</param>
public class NotFoundException(string entityName, object key)
    : AppException($"{entityName} with key '{key}' was not found.");
