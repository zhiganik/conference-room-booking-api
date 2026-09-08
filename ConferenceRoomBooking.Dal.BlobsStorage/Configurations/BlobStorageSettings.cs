namespace ConferenceRoomBooking.Dal.BlobsStorage.Configurations;

public class BlobStorageSettings
{
    public const string SectionName = "BlobStorage";

    public string AccountUrl { get; set; } = string.Empty;
}
