using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using ConferenceRoomBooking.Application.Orchestrators.ServiceOptions;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceOptionsController(IServiceOptionOrchestrator orchestrator) : ControllerBase
{
    /// <response code="201">Service created successfully.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceOptionResponse>> Create([FromBody] CreateServiceOptionRequest request,
        CancellationToken cancellationToken)
    {
        var serviceOption = await orchestrator.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { serviceOptionId = serviceOption.Id }, serviceOption);
    }
    
    /// <response code="200">Service found.</response>
    /// <response code="404">No service exists with the given ID.</response>
    [HttpGet("{serviceOptionId:int}")]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceOptionResponse>> GetById([FromRoute] int serviceOptionId,
        CancellationToken cancellationToken)
    {
        return await orchestrator.GetByIdAsync(serviceOptionId, cancellationToken);
    }
    
    /// <response code="200">Service updated successfully.</response>
    /// <response code="404">No service exists with the given ID.</response>
    [HttpPut("{serviceOptionId:int}")]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceOptionResponse>> Update([FromRoute] int serviceOptionId, 
        [FromBody] UpdateServiceOptionRequest request,
        CancellationToken cancellationToken)
    {
        return await orchestrator.UpdateAsync(serviceOptionId, request, cancellationToken);
    }
    
    /// <response code="204">Service deleted successfully.</response>
    /// <response code="404">No service exists with the given ID.</response>
    /// <response code="409">Service is currently linked to one or more rooms.</response>
    [HttpDelete("{serviceOptionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] int serviceOptionId, CancellationToken cancellationToken)
    {
        await orchestrator.DeleteAsync(serviceOptionId, cancellationToken);
        return NoContent();
    }
    
    /// <response code="200">Search completed (possibly with zero results).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceOptionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceOptionResponse>>> Search(
        [FromQuery] SearchServiceOptionsRequest request,
        CancellationToken cancellationToken)
    {
        var results = await orchestrator.SearchAsync(request, cancellationToken);
        return Ok(results);
    }
}