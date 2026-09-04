using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Web.Dtos.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Controllers;

/// <summary>Read-only business reports on room and service performance.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class AnalyticsController(IAnalyticsManager analyticsManager, IMapper mapper) : ControllerBase
{
    /// <summary>Per-room booking count, revenue, average duration, and revenue rank.</summary>
    /// <response code="200">Room performance report generated successfully.</response>
    [HttpGet("room-performance")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomPerformanceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomPerformanceResponse>>> GetRoomPerformance(
        CancellationToken cancellationToken)
    {
        var report = await analyticsManager.GetRoomPerformanceAsync(cancellationToken);
        return Ok(mapper.Map<List<RoomPerformanceResponse>>(report));
    }

    /// <summary>Per-service usage count, distinct rooms used in, revenue, and revenue rank.</summary>
    /// <response code="200">Service performance report generated successfully.</response>
    [HttpGet("service-performance")]
    [ProducesResponseType(typeof(IReadOnlyList<ServicePerformanceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServicePerformanceResponse>>> GetServicePerformance(
        CancellationToken cancellationToken)
    {
        var report = await analyticsManager.GetServicePerformanceAsync(cancellationToken);
        return Ok(mapper.Map<List<ServicePerformanceResponse>>(report));
    }
}
