namespace ConferenceRoomBooking.Bll.Common.Bookings.Models;

public class BookedServiceOption
{
    public int ServiceOptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAtBooking { get; set; }
}
