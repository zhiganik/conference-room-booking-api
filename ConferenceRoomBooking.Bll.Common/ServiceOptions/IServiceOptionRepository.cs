using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;

namespace ConferenceRoomBooking.Bll.Common.ServiceOptions;

public interface IServiceOptionRepository
{
    Task<ServiceOption> CreateAsync(ServiceOption serviceOption, CancellationToken cancellationToken);
    Task<ServiceOption?> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<ServiceOption?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceOption>> GetByIdsAsync(IReadOnlyCollection<int> serviceOptionIds, CancellationToken cancellationToken);
    Task UpdateAsync(ServiceOption serviceOption, CancellationToken cancellationToken);
    Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken);
    Task<bool> IsInUseByRoomAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, int? excludingId, CancellationToken cancellationToken);
}
