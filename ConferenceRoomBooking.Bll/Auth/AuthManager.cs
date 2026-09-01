using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Auth;

public class AuthManager(IUserRepository userRepository, IJwtIssuer jwtIssuer, IPasswordHasher passwordHasher) : IAuthManager
{
    public Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
