namespace ConferenceRoomBooking.Application.Exceptions;

public class RoomUnavailableException(string message) : AppException(message)
{
    
}