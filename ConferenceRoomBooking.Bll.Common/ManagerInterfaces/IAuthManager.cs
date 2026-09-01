using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.ManagerInterfaces;

public interface IAuthManager
{
    Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
