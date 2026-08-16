using ConferenceRoomBooking.Application.Dtos.Auth;
using ConferenceRoomBooking.Application.Orchestrators.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthOrchestrator authOrchestrator) : ControllerBase
{
    /// <response code="200">Registration succeeded — returns an access token.</response>
    /// <response code="409">Email already registered, or password doesn't meet requirements.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        return await authOrchestrator.RegisterAsync(request, cancellationToken);
    }

    /// <response code="200">Login succeeded — returns an access token.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        return await authOrchestrator.LoginAsync(request, cancellationToken);
    }
}