using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Settings;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Bll.Auth;

public class JwtIssuer(IOptions<JwtSettings> jwtOptions) : IJwtIssuer
{
    public string GenerateAccessToken(User user) => throw new NotImplementedException();
}
