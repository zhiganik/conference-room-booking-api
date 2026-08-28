using System;
namespace ConferenceRoomBooking.Application.Services
{
    public interface IUserContext
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
