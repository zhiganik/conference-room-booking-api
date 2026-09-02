using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Web.Dtos.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Controllers;

/// <summary>Booking creation and retrieval, with server-computed pricing.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireUser)]
public class BookingsController(IBookingManager bookingManager, IMapper mapper) : ControllerBase
{
    /// <summary>Books a room for the current user, optionally with extra services.</summary>
    /// <param name="request">The room, start time, duration, and selected service option ids.</param>
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
        var booking = await bookingManager.CreateAsync(request.RoomId, request.StartTime, request.DurationMinutes,
            request.ServiceOptionIds, cancellationToken);
        var response = mapper.Map<BookingResponse>(booking);
        return CreatedAtAction(nameof(GetById), new { bookingId = response.Id }, response);
    }

    /// <summary>Retrieves a single booking by ID.</summary>
    /// <param name="bookingId">The booking's id.</param>
    /// <response code="200">Booking found.</response>
    /// <response code="404">No booking exists with the given ID.</response>
    [HttpGet("{bookingId:int}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<ActionResult<BookingResponse>> GetById([FromRoute] int bookingId,
        CancellationToken cancellationToken)
    {
        var booking = await bookingManager.GetByIdAsync(bookingId, cancellationToken);
        return mapper.Map<BookingResponse>(booking);
    }

    /// <summary>Retrieves every booking made by the current user.</summary>
    /// <response code="200">Search completed (possibly with zero results).</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyList<BookingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var set = await bookingManager.GetByUserAsync(cancellationToken);
        return Ok(mapper.Map<List<BookingResponse>>(set));
    }
}
