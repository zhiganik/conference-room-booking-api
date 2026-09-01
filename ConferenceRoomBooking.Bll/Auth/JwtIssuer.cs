using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ConferenceRoomBooking.Bll.Auth;

public class JwtIssuer(IOptions<JwtSettings> jwtOptions) : IJwtIssuer
{
    public JwtToken GenerateAccessToken(User user)
    {
        var settings = jwtOptions.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtToken(accessToken, expiresAtUtc);
    }
}
