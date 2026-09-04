namespace ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;

public class ServiceOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
