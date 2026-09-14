namespace ConferenceRoomBooking.Utils.Storage;

public record BlobItemInfo(string Name, long? Size, DateTimeOffset? LastModified);
