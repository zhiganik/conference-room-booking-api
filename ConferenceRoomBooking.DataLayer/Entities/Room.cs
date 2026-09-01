namespace ConferenceRoomBooking.DataLayer.Entities;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    
    public ICollection<RoomServiceOption> RoomServiceOptions { get; set; } = new List<RoomServiceOption>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}