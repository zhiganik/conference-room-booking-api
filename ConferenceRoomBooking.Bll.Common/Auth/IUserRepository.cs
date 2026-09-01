using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Common.Auth;

public interface IUserRepository
{
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
