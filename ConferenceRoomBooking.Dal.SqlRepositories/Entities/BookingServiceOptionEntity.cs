namespace ConferenceRoomBooking.Dal.SqlRepositories.Entities;

public class BookingServiceOptionEntity
{
    public int BookingId { get; set; }
    public int ServiceOptionId { get; set; }
    public string ServiceOptionName { get; set; } = string.Empty;
    public decimal PriceAtBooking { get; set; }
}
