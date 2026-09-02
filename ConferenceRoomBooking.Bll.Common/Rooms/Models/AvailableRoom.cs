namespace ConferenceRoomBooking.Bll.Common.Rooms.Models;

public class AvailableRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
}
