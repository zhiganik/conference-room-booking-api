namespace ConferenceRoomBooking.DataLayer.Entities;

public class RoomServiceOption
{
    public int RoomId { get; set; }
    public int ServiceOptionId { get; set; }
    
    public Room Room { get; set; } = null!;
    public ServiceOption ServiceOption { get; set; } = null!;
}