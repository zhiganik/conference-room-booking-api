using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
