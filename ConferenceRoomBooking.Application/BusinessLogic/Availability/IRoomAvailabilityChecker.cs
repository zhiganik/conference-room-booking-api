namespace ConferenceRoomBooking.Application.BusinessLogic.Availability;

public interface IRoomAvailabilityChecker
{
    bool IsAvailable(DateTime startTime, DateTime endTime, IEnumerable<(DateTime Start, DateTime End)> existingBookings);
}