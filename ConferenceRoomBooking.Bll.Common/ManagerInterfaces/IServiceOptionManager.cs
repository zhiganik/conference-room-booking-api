using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.ManagerInterfaces;

public interface IServiceOptionManager
{
    Task<ServiceOption> CreateAsync(string name, decimal price, CancellationToken cancellationToken);
    Task<ServiceOption> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<ServiceOption> UpdateAsync(int serviceOptionId, string name, decimal price, CancellationToken cancellationToken);
    Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken);
}
