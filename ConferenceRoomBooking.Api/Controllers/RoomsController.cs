using ConferenceRoomBooking.Application.Dtos.Rooms;
using ConferenceRoomBooking.Application.Orchestrators.Rooms;
using ConferenceRoomBooking.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireUser)]
public class RoomsController(IRoomOrchestrator roomOrchestrator) : ControllerBase
{
    /// <summary>Creates a new conference room with its base rate and offered services.</summary>
    /// <response code="201">Room created successfully.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request, 
        CancellationToken cancellationToken)
    {
        var room = await roomOrchestrator.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetRoomById), new { roomId = room.Id }, room);
    }
    
    /// <summary>Retrieves a single room by ID.</summary>
    /// <response code="200">Room found.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpGet("{roomId:int}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetRoomById([FromRoute] int roomId, 
        CancellationToken cancellationToken)
    {
        var room = await roomOrchestrator.GetByIdAsync(roomId, cancellationToken);
        return room;
    }
    
    /// <summary>Updates an existing room's rate, capacity, and offered services.</summary>
    /// <response code="200">Room updated successfully.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpPut("{roomId:int}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> UpdateRoom([FromRoute] int roomId, [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room = await roomOrchestrator.UpdateAsync(roomId, request, cancellationToken);
        return room;
    }
    
    /// <summary>Soft-deletes a room.</summary>
    /// <response code="204">Room deleted successfully.</response>
    /// <response code="404">No room exists with the given ID.</response>
    [HttpDelete("{roomId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoom([FromRoute] int roomId, CancellationToken cancellationToken)
    {
        await roomOrchestrator.DeleteAsync(roomId, cancellationToken);
        return NoContent();
    }
    
    /// <summary>Searches for rooms available on a given date/time range with sufficient capacity.</summary>
    /// <response code="200">Search completed (possibly with zero results).</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailableRoomResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AvailableRoomResponse>>> SearchAvailableRooms(
        [FromQuery] SearchAvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        var rooms = await roomOrchestrator.SearchAvailableAsync(request, cancellationToken);
        return Ok(rooms);
    }
}