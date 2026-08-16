using ConferenceRoomBooking.Application.Dtos.Auth;

namespace ConferenceRoomBooking.Application.Orchestrators.Auth;

public interface IAuthOrchestrator
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}