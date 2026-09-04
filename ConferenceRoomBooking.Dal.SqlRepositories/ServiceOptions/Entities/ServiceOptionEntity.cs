namespace ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions.Entities;

public class ServiceOptionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
