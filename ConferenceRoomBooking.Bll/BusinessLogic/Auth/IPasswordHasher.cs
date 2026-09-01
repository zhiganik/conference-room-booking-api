namespace ConferenceRoomBooking.Bll.BusinessLogic.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
