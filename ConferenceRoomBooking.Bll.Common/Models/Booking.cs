namespace ConferenceRoomBooking.Bll.Common.Models;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal BaseRoomCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<BookedServiceOption> Services { get; set; } = [];
}
