namespace ConferenceRoomBooking.Bll.Common.Exceptions;

public class UnauthorizedException(string message) : AppException(message);
