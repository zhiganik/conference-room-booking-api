using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Auth;

/// <summary>
/// Issues signed JWT access tokens for authenticated users.
/// </summary>
public interface IJwtIssuer
{
    /// <summary>
    /// Builds and signs a new access token for the given user.
    /// </summary>
    /// <param name="user">The user to issue a token for. Its <see cref="User.Id"/>, <see cref="User.Email"/>,
    /// and <see cref="User.Role"/> are embedded as claims.</param>
    /// <returns>The signed token together with its expiry, derived from the token's own <c>exp</c> claim.</returns>
    JwtToken GenerateAccessToken(User user);
}

/// <summary>
/// A signed JWT access token and the UTC instant it expires at.
/// </summary>
/// <param name="AccessToken">The encoded JWT.</param>
/// <param name="ExpiresAtUtc">Expiry, read back from the token's <c>exp</c> claim.</param>
public record JwtToken(string AccessToken, DateTime ExpiresAtUtc);
