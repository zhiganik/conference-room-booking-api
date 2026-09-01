using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Auth;

public class UserRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IUserRepository
{
    public Task<User> CreateAsync(User user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
