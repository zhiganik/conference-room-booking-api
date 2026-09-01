using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Rooms;

public class RoomRepository(IDbConnectionFactory connectionFactory, IMapper mapper) : IRoomRepository
{
    public Task<Room> CreateAsync(Room room, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task UpdateAsync(Room room, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task SoftDeleteAsync(int roomId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(int capacity, DateTime startTime, DateTime endTime, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
