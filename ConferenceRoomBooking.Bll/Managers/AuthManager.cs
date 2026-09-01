using ConferenceRoomBooking.Bll.BusinessLogic.Auth;
using ConferenceRoomBooking.Bll.Common.ManagerInterfaces;
using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;

namespace ConferenceRoomBooking.Bll.Managers;

public class AuthManager(IUserRepository userRepository, IJwtIssuer jwtIssuer, IPasswordHasher passwordHasher) : IAuthManager
{
    public Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
