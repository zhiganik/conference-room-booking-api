namespace ConferenceRoomBooking.DataLayer.Entities;

public class ServiceOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public ICollection<RoomServiceOption> RoomServiceOptions { get; set; } = new List<RoomServiceOption>();
}