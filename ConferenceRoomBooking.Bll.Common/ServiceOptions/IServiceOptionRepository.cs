using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;

namespace ConferenceRoomBooking.Bll.Common.ServiceOptions;

/// <summary>
/// Persistence for the <c>ServiceOptions</c> catalog (Projector, Wi-Fi, Sound, etc.).
/// </summary>
public interface IServiceOptionRepository
{
    /// <summary>Inserts a new service option. <see cref="ServiceOption.Id"/> is database-generated and ignored on input.</summary>
    Task<ServiceOption> CreateAsync(ServiceOption serviceOption, CancellationToken cancellationToken);

    /// <summary>Looks up a service option by id.</summary>
    /// <returns>The matching service option, or <see langword="null"/> if no such id exists.</returns>
    Task<ServiceOption?> GetByIdAsync(Guid serviceOptionId, CancellationToken cancellationToken);

    /// <summary>Looks up a service option by name.</summary>
    /// <returns>The matching service option, or <see langword="null"/> if no service option has that name.</returns>
    Task<ServiceOption?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Looks up multiple service options by id in one round trip (via a table-valued parameter).
    /// </summary>
    /// <param name="serviceOptionIds">The ids to look up.</param>
    /// <returns>The service options that exist for the given ids. Missing ids are silently omitted —
    /// callers that need to detect them should compare the result's ids against the input.</returns>
    Task<IReadOnlyList<ServiceOption>> GetByIdsAsync(IReadOnlyCollection<Guid> serviceOptionIds, CancellationToken cancellationToken);

    /// <summary>Updates an existing service option's mutable fields (name, price).</summary>
    Task UpdateAsync(ServiceOption serviceOption, CancellationToken cancellationToken);

    /// <summary>Deletes a service option by id.</summary>
    Task DeleteAsync(Guid serviceOptionId, CancellationToken cancellationToken);

    /// <summary>Searches service options by a name substring.</summary>
    /// <param name="name">Substring to match against the name, or <see langword="null"/>/empty to return all.</param>
    /// <returns>Matching service options, ordered by name.</returns>
    Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken);

    /// <summary>Checks whether a service option is currently linked to any room, via <c>RoomServiceOptions</c>.</summary>
    Task<bool> IsInUseByRoomAsync(Guid serviceOptionId, CancellationToken cancellationToken);

    /// <summary>Checks whether a service option with the given name already exists.</summary>
    /// <param name="name">The name to check.</param>
    /// <param name="excludingId">When updating an existing service option, its own id — excluded from the
    /// check so renaming a service option to its current name doesn't report a conflict with itself.</param>
    Task<bool> ExistsByNameAsync(string name, Guid? excludingId, CancellationToken cancellationToken);
}
