namespace ConferenceRoomBooking.Bll.Common.Shared.Abstractions;

/// <summary>
/// The identity of the currently-authenticated caller. Implemented in the Web layer (reads the
/// current HTTP principal); consumed from Bll managers that need to know who's making the request.
/// </summary>
public interface IUserContext
{
    /// <summary>The current user's id (from the JWT <c>sub</c> claim), or <see langword="null"/> if unauthenticated.</summary>
    string? UserId { get; }

    /// <summary>Whether the current request carries a validated identity.</summary>
    bool IsAuthenticated { get; }
}
