using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;

namespace ConferenceRoomBooking.Bll.ServiceOptions;

public class ServiceOptionManager(IServiceOptionRepository serviceOptionRepository) : IServiceOptionManager
{
    public Task<ServiceOption> CreateAsync(string name, decimal price, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<ServiceOption> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<ServiceOption> UpdateAsync(int serviceOptionId, string name, decimal price, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
