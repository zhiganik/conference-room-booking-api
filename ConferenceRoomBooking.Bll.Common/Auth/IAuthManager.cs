using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Common.Auth;

public interface IAuthManager
{
    Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
