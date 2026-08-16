namespace ConferenceRoomBooking.Application.BusinessLogic.Availability;

public class RoomAvailabilityChecker : IRoomAvailabilityChecker
{
    public bool IsAvailable(DateTime startTime, DateTime endTime, IEnumerable<(DateTime Start, DateTime End)> existingBookings)
    {
        return existingBookings.All(b => !(startTime < b.End && b.Start < endTime));
    }
}