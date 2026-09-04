using ConferenceRoomBooking.Bll.Common.Auth.Models;

namespace ConferenceRoomBooking.Bll.Common.Auth;

/// <summary>
/// Persistence for the <c>Users</c> table.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Inserts a new user. <see cref="User.Id"/> is database-generated and ignored on input.
    /// </summary>
    /// <param name="user">The user to create. <see cref="User.PasswordHash"/> must already be hashed.</param>
    /// <returns>The created user, including its generated <see cref="User.Id"/>.</returns>
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Looks up a user by email.
    /// </summary>
    /// <param name="email">The email to look up.</param>
    /// <returns>The matching user, or <see langword="null"/> if no user has that email.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
