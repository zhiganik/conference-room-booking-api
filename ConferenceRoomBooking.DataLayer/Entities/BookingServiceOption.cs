namespace ConferenceRoomBooking.DataLayer.Entities;

public class BookingServiceOption
{
    public int BookingId { get; set; }
    public int ServiceOptionId { get; set; }
    public decimal PriceAtBooking { get; set; }
    
    public Booking Booking { get; set; } = null!;
    public ServiceOption ServiceOption { get; set; } = null!;
}