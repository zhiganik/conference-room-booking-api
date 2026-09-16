using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Bll.Rooms;

public class RoomManager(
    IRoomRepository roomRepository,
    IServiceOptionRepository serviceOptionRepository,
    IBookingRepository bookingRepository,
    ILogger<RoomManager> logger) : IRoomManager
{
    public async Task<Room> CreateAsync(string name, int capacity, decimal baseHourRate, List<Guid>? serviceOptionIds, CancellationToken cancellationToken)
    {
        var services = await ResolveServiceOptionsAsync(serviceOptionIds ?? [], cancellationToken);

        var room = new Room
        {
            Name = name,
            Capacity = capacity,
            BaseHourRate = baseHourRate,
            Services = services
        };

        var created = await roomRepository.CreateAsync(room, cancellationToken);
        logger.LogInformation("Room {RoomId} '{RoomName}' created.", created.Id, created.Name);
        return created;
    }

    public async Task<Room> GetByIdAsync(Guid roomId, CancellationToken cancellationToken) =>
        await roomRepository.GetByIdAsync(roomId, cancellationToken)
        ?? throw new NotFoundException(nameof(Room), roomId);

    public async Task<Room> UpdateAsync(Guid roomId, string name, int capacity, decimal baseHourRate, List<Guid>? serviceOptionIds, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var services = await ResolveServiceOptionsAsync(serviceOptionIds ?? [], cancellationToken);

        room.Name = name;
        room.Capacity = capacity;
        room.BaseHourRate = baseHourRate;
        room.Services = services;

        await roomRepository.UpdateAsync(room, cancellationToken);
        logger.LogInformation("Room {RoomId} updated.", roomId);

        return await roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);
    }

    public async Task DeleteAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        if (await bookingRepository.HasActiveForRoomAsync(roomId, DateTime.UtcNow, cancellationToken))
        {
            throw new ConflictException(
                "Room '{RoomName}' ({RoomId}) has active or upcoming bookings and cannot be deleted.",
                room.Name, roomId);
        }

        await roomRepository.SoftDeleteAsync(roomId, cancellationToken);
        logger.LogInformation("Room {RoomId} '{RoomName}' deleted.", roomId, room.Name);
    }

    public async Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(DateTime startDate, DateTime endDate, int capacity, CancellationToken cancellationToken) =>
        await roomRepository.SearchAvailableAsync(capacity, startDate, endDate, cancellationToken);

    private async Task<IEnumerable<ServiceOption>> ResolveServiceOptionsAsync(IReadOnlyCollection<Guid> serviceOptionIds, CancellationToken cancellationToken)
    {
        if (serviceOptionIds.Count == 0)
        {
            return [];
        }

        var existing = await serviceOptionRepository.GetByIdsAsync(serviceOptionIds, cancellationToken);

        if (existing.Count != serviceOptionIds.Count)
        {
            var missingIds = serviceOptionIds.Except(existing.Select(s => s.Id));
            throw new NotFoundException(nameof(ServiceOption), string.Join(", ", missingIds));
        }

        return existing;
    }
}
