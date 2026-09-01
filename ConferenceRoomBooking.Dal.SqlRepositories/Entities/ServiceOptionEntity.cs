namespace ConferenceRoomBooking.Dal.SqlRepositories.Entities;

public class ServiceOptionEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
