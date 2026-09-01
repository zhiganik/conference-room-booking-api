namespace ConferenceRoomBooking.Application.Exceptions;

public class UnauthorizedException(string message) : AppException(message);