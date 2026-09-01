namespace ConferenceRoomBooking.Bll.Common.Shared.Security;

public static class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";

    public static IEnumerable<string> Get() => [User, Admin];
}
