namespace ConferenceRoomBooking.Application.Exceptions;

public class ConflictException(string message) : AppException(message);