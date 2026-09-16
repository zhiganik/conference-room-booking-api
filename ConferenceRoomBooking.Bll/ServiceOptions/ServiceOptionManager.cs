using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Bll.ServiceOptions;

public class ServiceOptionManager(
    IServiceOptionRepository serviceOptionRepository,
    ILogger<ServiceOptionManager> logger) : IServiceOptionManager
{
    public async Task<ServiceOption> CreateAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);

        var serviceOption = new ServiceOption
        {
            Name = name,
            Price = price
        };

        var created = await serviceOptionRepository.CreateAsync(serviceOption, cancellationToken);
        logger.LogInformation("Service option {ServiceOptionId} '{Name}' created.", created.Id, created.Name);
        return created;
    }

    public async Task<ServiceOption> GetByIdAsync(Guid serviceOptionId, CancellationToken cancellationToken) =>
        await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
        ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

    public async Task<ServiceOption> UpdateAsync(Guid serviceOptionId, string name, decimal price, CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

        await EnsureNameIsUniqueAsync(name, serviceOptionId, cancellationToken);

        serviceOption.Name = name;
        serviceOption.Price = price;

        await serviceOptionRepository.UpdateAsync(serviceOption, cancellationToken);
        logger.LogInformation("Service option {ServiceOptionId} updated.", serviceOptionId);

        return serviceOption;
    }

    public async Task DeleteAsync(Guid serviceOptionId, CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionRepository.GetByIdAsync(serviceOptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);

        if (await serviceOptionRepository.IsInUseByRoomAsync(serviceOptionId, cancellationToken))
        {
            throw new ConflictException(
                "ServiceOption '{Name}' ({ServiceOptionId}) is linked to one or more rooms and cannot be deleted.",
                serviceOption.Name, serviceOptionId);
        }

        await serviceOptionRepository.DeleteAsync(serviceOptionId, cancellationToken);
        logger.LogInformation("Service option {ServiceOptionId} '{Name}' deleted.", serviceOptionId, serviceOption.Name);
    }

    public async Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken) =>
        await serviceOptionRepository.SearchAsync(name, cancellationToken);

    private async Task EnsureNameIsUniqueAsync(string name, Guid? excludingId, CancellationToken cancellationToken)
    {
        if (await serviceOptionRepository.ExistsByNameAsync(name, excludingId, cancellationToken))
        {
            throw new ConflictException("A service named '{Name}' already exists.", name);
        }
    }
}
