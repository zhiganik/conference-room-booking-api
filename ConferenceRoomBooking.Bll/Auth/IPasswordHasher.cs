namespace ConferenceRoomBooking.Bll.Auth;

/// <summary>
/// PBKDF2 (<see cref="System.Security.Cryptography.Rfc2898DeriveBytes"/>) based password hasher,
/// replacing ASP.NET Identity's <c>PasswordHasher&lt;TUser&gt;</c> now that <c>Users</c> is a plain
/// custom table.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password into a self-describing string (iteration count, salt, and hash
    /// all encoded together) suitable for storing in <c>Users.PasswordHash</c>.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>The encoded hash string.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Checks a plaintext password against a previously-hashed value, using a constant-time compare.
    /// </summary>
    /// <param name="password">The plaintext password supplied by the caller.</param>
    /// <param name="passwordHash">The encoded hash previously produced by <see cref="HashPassword"/>.</param>
    /// <returns><see langword="true"/> if the password matches the hash.</returns>
    bool VerifyPassword(string password, string passwordHash);
}
