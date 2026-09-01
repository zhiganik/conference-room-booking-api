namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

public class NotFoundException(string entityName, object key)
    : AppException($"{entityName} with key '{key}' was not found.");
