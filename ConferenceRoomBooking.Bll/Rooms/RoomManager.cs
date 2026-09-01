using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;

namespace ConferenceRoomBooking.Bll.Rooms;

public class RoomManager(IRoomRepository roomRepository, IServiceOptionRepository serviceOptionRepository, IBookingRepository bookingRepository) : IRoomManager
{
    public Task<Room> CreateAsync(string name, int capacity, decimal baseHourRate, List<int>? serviceOptionIds, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Room> GetByIdAsync(int roomId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Room> UpdateAsync(int roomId, string name, int capacity, decimal baseHourRate, List<int>? serviceOptionIds, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task DeleteAsync(int roomId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<AvailableRoom>> SearchAvailableAsync(DateTime startDate, DateTime endDate, int capacity, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
