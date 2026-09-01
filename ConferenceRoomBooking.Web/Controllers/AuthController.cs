using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Web.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController(IAuthManager authManager, IMapper mapper) : ControllerBase
{
    /// <response code="200">Registration succeeded — returns an access token.</response>
    /// <response code="409">Email already registered, or password doesn't meet requirements.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authManager.RegisterAsync(request.Email, request.Password, cancellationToken);
        return mapper.Map<AuthResponse>(result);
    }

    /// <response code="200">Login succeeded — returns an access token.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authManager.LoginAsync(request.Email, request.Password, cancellationToken);
        return mapper.Map<AuthResponse>(result);
    }
}
