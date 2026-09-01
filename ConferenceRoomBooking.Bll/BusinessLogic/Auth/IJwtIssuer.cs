using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.BusinessLogic.Auth;

public interface IJwtIssuer
{
    string GenerateAccessToken(User user);
}
