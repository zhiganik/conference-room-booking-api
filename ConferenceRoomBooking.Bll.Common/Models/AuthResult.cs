namespace ConferenceRoomBooking.Bll.Common.Models;

public record AuthResult(string AccessToken, DateTime ExpiresAtUtc, Guid UserId, string Email);
