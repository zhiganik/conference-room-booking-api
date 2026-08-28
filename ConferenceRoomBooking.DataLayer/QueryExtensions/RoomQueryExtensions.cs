using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.DataLayer.QueryExtensions;

public static class RoomQueryExtensions
{
    public static IQueryable<Room> WithCapacityAtLeast(this IQueryable<Room> query, int capacity) =>
        query.Where(r => r.Capacity >= capacity);

    public static IQueryable<Room> WithoutOverlappingBookings(this IQueryable<Room> query, DateTime startTime, DateTime endTime) =>
        query.Where(r => !r.Bookings.Any(b => b.StartTime < endTime && b.EndTime > startTime));
}