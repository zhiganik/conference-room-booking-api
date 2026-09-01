namespace ConferenceRoomBooking.Web.Dtos.Auth;

public record AuthResponse(string AccessToken, DateTime ExpiresAt, AppUserResponse AppUser);