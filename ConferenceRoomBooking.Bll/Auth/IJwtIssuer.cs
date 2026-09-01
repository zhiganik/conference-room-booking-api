using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Auth;

public interface IJwtIssuer
{
    JwtToken GenerateAccessToken(User user);
}

public record JwtToken(string AccessToken, DateTime ExpiresAtUtc);
