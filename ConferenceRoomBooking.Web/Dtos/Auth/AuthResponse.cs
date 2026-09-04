namespace ConferenceRoomBooking.Web.Dtos.Auth;

public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, AppUserResponse AppUser);