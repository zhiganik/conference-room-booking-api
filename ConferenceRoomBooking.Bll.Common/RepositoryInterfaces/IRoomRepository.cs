using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.RepositoryInterfaces;

public interface IRoomRepository
{
    Task<Room> CreateAsync(Room room, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken);
    Task UpdateAsync(Room room, CancellationToken cancellationToken);
    Task SoftDeleteAsync(int roomId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(int capacity, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}
