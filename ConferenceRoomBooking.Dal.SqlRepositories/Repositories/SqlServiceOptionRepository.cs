using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Models;
using ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;
using ConferenceRoomBooking.Dal.SqlRepositories.Formatters;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Repositories;

public class SqlServiceOptionRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IServiceOptionRepository
{
    public Task<ServiceOption> CreateAsync(ServiceOption serviceOption, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<ServiceOption?> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServiceOption>> GetByIdsAsync(IReadOnlyCollection<int> serviceOptionIds, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task UpdateAsync(ServiceOption serviceOption, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<bool> IsInUseByRoomAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<bool> ExistsByNameAsync(string name, int? excludingId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
