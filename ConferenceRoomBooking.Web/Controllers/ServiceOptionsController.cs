using AutoMapper;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Web.Dtos.ServiceOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Web.Controllers;

/// <summary>CRUD for the service option catalog offered by rooms (Projector, Wi-Fi, Sound, etc.).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class ServiceOptionsController(IServiceOptionManager serviceOptionManager, IMapper mapper) : ControllerBase
{
    /// <summary>Creates a new service option.</summary>
    /// <param name="request">The service option's name and price.</param>
    /// <response code="201">Service created successfully.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceOptionResponse>> Create([FromBody] CreateServiceOptionRequest request,
        CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionManager.CreateAsync(request.Name, request.Price, cancellationToken);
        var response = mapper.Map<ServiceOptionResponse>(serviceOption);
        return CreatedAtAction(nameof(GetById), new { serviceOptionId = response.Id }, response);
    }

    /// <summary>Retrieves a single service option by ID.</summary>
    /// <param name="serviceOptionId">The service option's id.</param>
    /// <response code="200">Service found.</response>
    /// <response code="404">No service exists with the given ID.</response>
    [HttpGet("{serviceOptionId:int}")]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceOptionResponse>> GetById([FromRoute] int serviceOptionId,
        CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionManager.GetByIdAsync(serviceOptionId, cancellationToken);
        return mapper.Map<ServiceOptionResponse>(serviceOption);
    }

    /// <summary>Updates an existing service option's name and price.</summary>
    /// <param name="serviceOptionId">The service option's id.</param>
    /// <param name="request">The service option's new name and price.</param>
    /// <response code="200">Service updated successfully.</response>
    /// <response code="404">No service exists with the given ID.</response>
    [HttpPut("{serviceOptionId:int}")]
    [ProducesResponseType(typeof(ServiceOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceOptionResponse>> Update([FromRoute] int serviceOptionId,
        [FromBody] UpdateServiceOptionRequest request,
        CancellationToken cancellationToken)
    {
        var serviceOption = await serviceOptionManager.UpdateAsync(serviceOptionId, request.Name, request.Price, cancellationToken);
        return mapper.Map<ServiceOptionResponse>(serviceOption);
    }

    /// <summary>Deletes a service option.</summary>
    /// <param name="serviceOptionId">The service option's id.</param>
    /// <response code="204">Service deleted successfully.</response>
    /// <response code="404">No service exists with the given ID.</response>
    /// <response code="409">Service is currently linked to one or more rooms.</response>
    [HttpDelete("{serviceOptionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] int serviceOptionId, CancellationToken cancellationToken)
    {
        await serviceOptionManager.DeleteAsync(serviceOptionId, cancellationToken);
        return NoContent();
    }

    /// <summary>Searches service options by a name substring.</summary>
    /// <param name="request">The name substring to search for, or empty to return all.</param>
    /// <response code="200">Search completed (possibly with zero results).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceOptionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceOptionResponse>>> Search(
        [FromQuery] SearchServiceOptionsRequest request,
        CancellationToken cancellationToken)
    {
        var results = await serviceOptionManager.SearchAsync(request.Name, cancellationToken);
        return Ok(mapper.Map<List<ServiceOptionResponse>>(results));
    }
}
