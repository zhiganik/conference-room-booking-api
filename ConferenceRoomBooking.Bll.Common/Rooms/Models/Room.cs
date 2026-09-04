using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;

namespace ConferenceRoomBooking.Bll.Common.Rooms.Models;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourRate { get; set; }
    public IEnumerable<ServiceOption> Services { get; set; } = [];
}
