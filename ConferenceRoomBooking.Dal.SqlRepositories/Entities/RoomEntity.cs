namespace ConferenceRoomBooking.Dal.SqlRepositories.Entities;

public class RoomEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourRate { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public List<ServiceOptionEntity> ServiceOptions { get; set; } = [];
}
