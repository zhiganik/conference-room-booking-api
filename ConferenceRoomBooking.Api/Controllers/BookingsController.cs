using ConferenceRoomBooking.Application.Dtos.Booking;
using ConferenceRoomBooking.Application.Orchestrators.Booking;
using ConferenceRoomBooking.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireUser)]
public class BookingsController(IBookingOrchestrator orchestrator) : ControllerBase
{
    /// <response code="201">Booking created — includes calculated total cost.</response>
    /// <response code="404">Room or a selected service doesn't exist.</response>
    /// <response code="409">Room is already booked for the requested window.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await orchestrator.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { bookingId = booking.Id }, booking);
    }

    /// <response code="200">Booking found.</response>
    /// <response code="404">No booking exists with the given ID.</response>
    [HttpGet("{bookingId:int}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById([FromRoute] int bookingId,
        CancellationToken cancellationToken)
    {
        return await orchestrator.GetByIdAsync(bookingId, cancellationToken);
    }
}