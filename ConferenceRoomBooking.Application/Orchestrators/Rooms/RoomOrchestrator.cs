using ConferenceRoomBooking.Application.Dtos.Rooms;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Application.Mappers;
using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using ConferenceRoomBooking.DataLayer.QueryExtensions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Orchestrators.Rooms;

public class RoomOrchestrator(AppDbContext dbContext) : IRoomOrchestrator
{
    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var serviceOptionIds = request.ServiceOptionIds ?? [];
        
        await EnsureServiceOptionsExistAsync(serviceOptionIds, cancellationToken);
        
        var room = new Room
        {
            Name = request.Name,
            Capacity = request.Capacity,
            BaseHourRate = request.BaseHourRate,
            RoomServiceOptions = serviceOptionIds
                .Select(id => new RoomServiceOption { ServiceOptionId = id })
                .ToList()
        };
        
        dbContext.Rooms.Add(room);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(room.Id, cancellationToken);
    }

    public async Task<RoomResponse> GetByIdAsync(int roomId, CancellationToken cancellationToken)
    {
        var result = await dbContext.Rooms
            .Where(r => r.Id == roomId)
            .ToResponse()
            .FirstOrDefaultAsync(cancellationToken);
        
        return result ?? throw new NotFoundException(nameof(Room), roomId);
    }

    public async Task<RoomResponse> UpdateAsync(int roomId, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await dbContext.Rooms
            .Include(r => r.RoomServiceOptions) 
            .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
        
        if (room is null) throw new NotFoundException(nameof(Room), roomId);
        
        var serviceOptionIds = request.ServiceOptionIds ?? [];
        await EnsureServiceOptionsExistAsync(serviceOptionIds, cancellationToken);
        
        room.Name = request.Name;
        room.Capacity = request.Capacity;
        room.BaseHourRate = request.BaseHourRate;
        room.RoomServiceOptions.Clear();
        
        foreach (var serviceOptionId in serviceOptionIds)
        {
            room.RoomServiceOptions.Add(new RoomServiceOption
            {
                RoomId = room.Id,
                ServiceOptionId = serviceOptionId
            });
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(room.Id, cancellationToken);
    }

    public async Task DeleteAsync(int roomId, CancellationToken cancellationToken)
    {
        var room = await dbContext.Rooms
            .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

        if (room is null)
            throw new NotFoundException(nameof(Room), roomId);
        
        var hasActiveOrUpcomingBookings = await dbContext.Bookings
            .ForRoom(roomId)
            .AnyAsync(b => b.EndTime > DateTime.UtcNow, cancellationToken);

        if (hasActiveOrUpcomingBookings)
            throw new ConflictException($"Room '{room.Name}' has active or upcoming bookings and cannot be deleted.");
        
        room.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableRoomResponse>> SearchAvailableAsync(SearchAvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        return await dbContext.Rooms
            .WithCapacityAtLeast(request.Capacity)
            .OrderBy(r => r.Name)
            .ToAvailableResponse()
            .ToListAsync(cancellationToken);
    }
    
    private async Task EnsureServiceOptionsExistAsync(List<int> serviceOptionIds, CancellationToken cancellationToken)
    {
        if (serviceOptionIds.Count == 0)
        {
            return;
        }

        var existingIds = await dbContext.ServiceOptions
            .Where(s => serviceOptionIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var missingIds = serviceOptionIds.Except(existingIds).ToList();
        
        if (missingIds.Count > 0)
        {
            throw new NotFoundException(nameof(ServiceOption), string.Join(", ", missingIds));
        }
    }
}