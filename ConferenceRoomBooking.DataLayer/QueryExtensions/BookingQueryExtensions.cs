using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.DataLayer.QueryExtensions;

public static class BookingQueryExtensions
{
    public static IQueryable<Booking> ForRoom(this IQueryable<Booking> query, int roomId) =>
        query.Where(b => b.RoomId == roomId);

    public static IQueryable<Booking> Overlapping(this IQueryable<Booking> query, DateTime startTime, DateTime endTime) =>
        query.Where(b => b.StartTime < endTime && b.EndTime > startTime);
}