namespace ConferenceRoomBooking.DataLayer.Entities;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public required string RoomName { get; set; }
    public required string UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal BaseRoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Room Room { get; set; } = null!;
    public ICollection<BookingServiceOption> BookingServiceOptions { get; set; } = new List<BookingServiceOption>();
}