using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Bll.Auth;

public class AuthManager(
    IUserRepository userRepository,
    IJwtIssuer jwtIssuer,
    IPasswordHasher passwordHasher,
    ILogger<AuthManager> logger) : IAuthManager
{
    public async Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("Email '{Email}' is already registered.", email);
        }

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHasher.HashPassword(password),
            Role = Roles.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await userRepository.CreateAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} registered with email {Email}.", created.Id, created.Email);
        return BuildAuthResult(created);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedException("Login failed for '{Email}': invalid email or password.", email);
        }

        logger.LogInformation("User {UserId} logged in.", user.Id);
        return BuildAuthResult(user);
    }

    private AuthResult BuildAuthResult(User user)
    {
        var token = jwtIssuer.GenerateAccessToken(user);
        return new AuthResult(token.AccessToken, token.ExpiresAtUtc, user.Id, user.Email);
    }
}
