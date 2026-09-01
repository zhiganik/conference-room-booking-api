namespace ConferenceRoomBooking.Bll.Common.Abstractions;

// Implemented in the Web layer (reads the current HTTP principal); consumed from Bll managers.
public interface IUserContext
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}
