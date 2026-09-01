using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;
using ConferenceRoomBooking.Dal.SqlRepositories.Formatters;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Repositories;

public class SqlUserRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IUserRepository
{
    public Task<User> CreateAsync(User user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
