namespace ConferenceRoomBooking.Dal.SqlRepositories.Bookings.Entities;

public class BookingServiceOptionEntity
{
    public Guid BookingId { get; set; }
    public Guid ServiceOptionId { get; set; }
    public string ServiceOptionName { get; set; } = string.Empty;
    public decimal PriceAtBooking { get; set; }
}
