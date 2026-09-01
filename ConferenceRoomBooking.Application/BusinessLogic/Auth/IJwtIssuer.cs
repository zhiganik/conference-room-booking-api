using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.Application.BusinessLogic.Auth;

public interface IJwtIssuer
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
}