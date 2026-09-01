namespace ConferenceRoomBooking.Bll.Common.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourRate { get; set; }
    public List<ServiceOption> Services { get; set; } = [];
}
