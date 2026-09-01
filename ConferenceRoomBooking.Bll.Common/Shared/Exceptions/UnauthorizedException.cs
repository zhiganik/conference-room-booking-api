namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

public class UnauthorizedException(string message) : AppException(message);
