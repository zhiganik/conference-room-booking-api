using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;

namespace ConferenceRoomBooking.Functions.Services;

/// <summary>
/// Functions run outside of an authenticated HTTP request (timers, webhooks), so there is no
/// caller identity to expose.
/// </summary>
public class SystemUserContext : IUserContext
{
    public string? UserId => null;

    public bool IsAuthenticated => false;
}
