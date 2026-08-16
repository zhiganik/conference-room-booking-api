using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.DataLayer.QueryExtensions;

public static class BookingQueryExtensions
{
    public static IQueryable<Booking> ForRoom(this IQueryable<Booking> query, int roomId) =>
        query.Where(b => b.RoomId == roomId);
}