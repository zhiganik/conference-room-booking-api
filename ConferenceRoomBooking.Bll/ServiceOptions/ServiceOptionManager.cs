using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.ServiceOptions;

public class ServiceOptionManager(IServiceOptionRepository serviceOptionRepository) : IServiceOptionManager
{
    public async Task<ServiceOption> CreateAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);

        var serviceOption = new ServiceOption
        {
            Name = name,
            Price = price
        };

        return await serviceOptionRepository.CreateAsync(serviceOption, cancellationToken);
    }

    public async Task<ServiceOption> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken) =>
        await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
        ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

    public async Task<ServiceOption> UpdateAsync(int serviceOptionId, string name, decimal price, CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

        await EnsureNameIsUniqueAsync(name, serviceOptionId, cancellationToken);

        serviceOption.Name = name;
        serviceOption.Price = price;

        await serviceOptionRepository.UpdateAsync(serviceOption, cancellationToken);

        return serviceOption;
    }

    public async Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

        if (await serviceOptionRepository.IsInUseByRoomAsync(serviceOptionId, cancellationToken))
        {
            throw new ConflictException(
                $"ServiceOption '{serviceOption.Name}' is linked to one or more rooms and cannot be deleted.");
        }

        await serviceOptionRepository.DeleteAsync(serviceOptionId, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken) =>
        await serviceOptionRepository.SearchAsync(name, cancellationToken);

    private async Task EnsureNameIsUniqueAsync(string name, int? excludingId, CancellationToken cancellationToken)
    {
        if (await serviceOptionRepository.ExistsByNameAsync(name, excludingId, cancellationToken))
        {
            throw new ConflictException($"A service named '{name}' already exists.");
        }
    }
}
