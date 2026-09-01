using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.Settings;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Bll.BusinessLogic.Auth;

public class JwtIssuer(IOptions<JwtSettings> jwtOptions) : IJwtIssuer
{
    public string GenerateAccessToken(User user) => throw new NotImplementedException();
}
