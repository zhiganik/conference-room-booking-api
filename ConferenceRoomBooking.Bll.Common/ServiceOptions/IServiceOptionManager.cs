using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.ServiceOptions;

/// <summary>
/// Business rules for the <c>ServiceOptions</c> catalog: name uniqueness, and refusing to delete a
/// service option that's still offered by a room.
/// </summary>
public interface IServiceOptionManager
{
    /// <summary>Creates a new service option.</summary>
    /// <exception cref="ConflictException">A service option with this name already exists.</exception>
    Task<ServiceOption> CreateAsync(string name, decimal price, CancellationToken cancellationToken);

    /// <summary>Retrieves a service option by id.</summary>
    /// <exception cref="NotFoundException">No service option exists with the given id.</exception>
    Task<ServiceOption> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken);

    /// <summary>Updates an existing service option's name and price.</summary>
    /// <exception cref="NotFoundException">No service option exists with the given id.</exception>
    /// <exception cref="ConflictException">Another service option already has the new name.</exception>
    Task<ServiceOption> UpdateAsync(int serviceOptionId, string name, decimal price, CancellationToken cancellationToken);

    /// <summary>Deletes a service option.</summary>
    /// <exception cref="NotFoundException">No service option exists with the given id.</exception>
    /// <exception cref="ConflictException">The service option is still offered by one or more rooms.</exception>
    Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken);

    /// <summary>Searches service options by a name substring.</summary>
    /// <param name="name">Substring to match against the name, or <see langword="null"/>/empty to return all.</param>
    Task<IReadOnlyList<ServiceOption>> SearchAsync(string? name, CancellationToken cancellationToken);
}
