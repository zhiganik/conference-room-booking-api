namespace ConferenceRoomBooking.Bll.Common.Auth.Models;

public record AuthResult(string AccessToken, DateTime ExpiresAtUtc, Guid UserId, string Email);
