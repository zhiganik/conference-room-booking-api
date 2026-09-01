using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;
using ConferenceRoomBooking.Bll.Common.Shared.Security;

namespace ConferenceRoomBooking.Bll.Auth;

public class AuthManager(IUserRepository userRepository, IJwtIssuer jwtIssuer, IPasswordHasher passwordHasher) : IAuthManager
{
    public async Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"Email '{email}' is already registered.");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHasher.HashPassword(password),
            Role = Roles.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await userRepository.CreateAsync(user, cancellationToken);
        return BuildAuthResult(created);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return BuildAuthResult(user);
    }

    private AuthResult BuildAuthResult(User user)
    {
        var token = jwtIssuer.GenerateAccessToken(user);
        return new AuthResult(token.AccessToken, token.ExpiresAtUtc, user.Id, user.Email);
    }
}
