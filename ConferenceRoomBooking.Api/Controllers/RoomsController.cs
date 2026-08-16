using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Room A", "Room B", "Room C" });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(new { Id = id, Name = "Room A" });
    }

    [HttpPost]
    public IActionResult Create([FromBody] string name)
    {
        return Created(string.Empty, new { Name = name });
    }
}