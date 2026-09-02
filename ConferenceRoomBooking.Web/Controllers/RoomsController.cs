using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Web.Dtos.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Controllers;

/// <summary>Conference room CRUD and availability search.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class RoomsController(IRoomManager roomManager, IMapper mapper) : ControllerBase
{
    /// <summary>Creates a new conference room with its base rate and offered services.</summary>
    /// <param name="request">The room's name, capacity, base hourly rate, and offered service option ids.</param>
    /// <response code="201">Room created successfully.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room = await roomManager.CreateAsync(request.Name, request.Capacity, request.BaseHourRate,
            request.ServiceOptionIds, cancellationToken);
        var response = mapper.Map<RoomResponse>(room);
        return CreatedAtAction(nameof(GetRoomById), new { roomId = response.Id }, response);
    }

    /// <summary>Retrieves a single room by ID.</summary>
    /// <param name="roomId">The room's id.</param>
    /// <response code="200">Room found.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpGet("{roomId:guid}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetRoomById([FromRoute] Guid roomId,
        CancellationToken cancellationToken)
    {
        var room = await roomManager.GetByIdAsync(roomId, cancellationToken);
        return mapper.Map<RoomResponse>(room);
    }

    /// <summary>Updates an existing room's rate, capacity, and offered services.</summary>
    /// <param name="roomId">The room's id.</param>
    /// <param name="request">The room's new name, capacity, base hourly rate, and offered service option ids.</param>
    /// <response code="200">Room updated successfully.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpPut("{roomId:guid}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> UpdateRoom([FromRoute] Guid roomId, [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room = await roomManager.UpdateAsync(roomId, request.Name, request.Capacity, request.BaseHourRate,
            request.ServiceOptionIds, cancellationToken);
        return mapper.Map<RoomResponse>(room);
    }

    /// <summary>Soft-deletes a room.</summary>
    /// <param name="roomId">The room's id.</param>
    /// <response code="204">Room deleted successfully.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpDelete("{roomId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoom([FromRoute] Guid roomId, CancellationToken cancellationToken)
    {
        await roomManager.DeleteAsync(roomId, cancellationToken);
        return NoContent();
    }

    /// <summary>Searches for rooms available on a given date/time range with sufficient capacity.</summary>
    /// <param name="request">The requested date/time window and minimum capacity.</param>
    /// <response code="200">Search completed (possibly with zero results).</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailableRoomResponse>), StatusCodes.Status200OK)]
    [Authorize(Policy = AuthorizationPolicies.RequireUser)]
    public async Task<ActionResult<IReadOnlyList<AvailableRoomResponse>>> SearchAvailableRooms(
        [FromQuery] SearchAvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        var rooms = await roomManager.SearchAvailableAsync(request.StartDate, request.EndDate, request.Capacity, cancellationToken);
        return Ok(mapper.Map<List<AvailableRoomResponse>>(rooms));
    }
}
