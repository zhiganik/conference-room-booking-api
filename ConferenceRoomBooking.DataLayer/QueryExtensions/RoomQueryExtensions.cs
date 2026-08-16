using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.DataLayer.QueryExtensions;

public static class RoomQueryExtensions
{
    public static IQueryable<Room> WithCapacityAtLeast(this IQueryable<Room> query, int capacity) =>
        query.Where(r => r.Capacity >= capacity);
}