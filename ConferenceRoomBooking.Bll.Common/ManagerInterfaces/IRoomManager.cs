using ConferenceRoomBooking.Bll.Common.Models;

namespace ConferenceRoomBooking.Bll.Common.ManagerInterfaces;

public interface IRoomManager
{
    Task<Room> CreateAsync(string name, int capacity, decimal baseHourRate, List<int>? serviceOptionIds, CancellationToken cancellationToken);
    Task<Room> GetByIdAsync(int roomId, CancellationToken cancellationToken);
    Task<Room> UpdateAsync(int roomId, string name, int capacity, decimal baseHourRate, List<int>? serviceOptionIds, CancellationToken cancellationToken);
    Task DeleteAsync(int roomId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(DateTime startDate, DateTime endDate, int capacity, CancellationToken cancellationToken);
}
