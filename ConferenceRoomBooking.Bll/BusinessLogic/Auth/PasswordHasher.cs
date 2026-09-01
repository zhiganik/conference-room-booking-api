namespace ConferenceRoomBooking.Bll.BusinessLogic.Auth;

// PBKDF2 (Rfc2898DeriveBytes) based hasher, replacing ASP.NET Identity's PasswordHasher<TUser>.
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => throw new NotImplementedException();

    public bool VerifyPassword(string password, string passwordHash) => throw new NotImplementedException();
}
