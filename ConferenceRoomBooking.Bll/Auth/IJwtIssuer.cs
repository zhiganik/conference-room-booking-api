using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Auth;

public interface IJwtIssuer
{
    string GenerateAccessToken(User user);
}
