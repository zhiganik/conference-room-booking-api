using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Auth;

/// <summary>
/// Registration and login. Both operations issue a fresh JWT access token on success.
/// </summary>
public interface IAuthManager
{
    /// <summary>
    /// Creates a new account with the <c>User</c> role and returns a signed-in result.
    /// </summary>
    /// <param name="email">The account email. Must not already be registered.</param>
    /// <param name="password">The plaintext password to hash and store.</param>
    /// <exception cref="ConflictException">A user with this email is already registered.</exception>
    Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Authenticates an existing account.
    /// </summary>
    /// <param name="email">The account email.</param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <exception cref="UnauthorizedException">The email/password combination is invalid.</exception>
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
