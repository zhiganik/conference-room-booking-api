using ConferenceRoomBooking.Application.Dtos.Analytics;
using ConferenceRoomBooking.Application.Orchestrators.Analytics;
using ConferenceRoomBooking.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class AnalyticsController(IAnalyticsOrchestrator orchestrator) : ControllerBase
{
    /// <summary>Per-room booking count, revenue, average duration, and revenue rank.</summary>
    /// <response code="200">Room performance report generated successfully.</response>
    [HttpGet("room-performance")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomPerformanceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomPerformanceResponse>>> GetRoomPerformance(
        CancellationToken cancellationToken)
    {
        return Ok(await orchestrator.GetRoomPerformanceAsync(cancellationToken));
    }

    /// <summary>Per-service usage count, distinct rooms used in, revenue, and revenue rank.</summary>
    /// <response code="200">Service performance report generated successfully.</response>
    [HttpGet("service-performance")]
    [ProducesResponseType(typeof(IReadOnlyList<ServicePerformanceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServicePerformanceResponse>>> GetServicePerformance(
        CancellationToken cancellationToken)
    {
        return Ok(await orchestrator.GetServicePerformanceAsync(cancellationToken));
    }
}