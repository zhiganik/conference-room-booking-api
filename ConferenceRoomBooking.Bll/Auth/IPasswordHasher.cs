namespace ConferenceRoomBooking.Bll.Auth;

public interface IPasswordHasher
{
    /// <summary>
    /// PBKDF2 (Rfc2898DeriveBytes) based hasher, replacing ASP.NET Identity's PasswordHasher<TUser>.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    string HashPassword(string password);

    /// <summary>
    /// Matching client password with hashed value
    /// </summary>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    bool VerifyPassword(string password, string passwordHash);
}
