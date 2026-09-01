using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Common.Bookings.Exceptions;

public class RoomUnavailableException(string message) : AppException(message)
{

}
