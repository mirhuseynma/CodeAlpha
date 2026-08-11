using EventRegistrationSystem.Application.Features.Registrations.Commands.CancelRegistration;
using EventRegistrationSystem.Application.Features.Registrations.Commands.RegisterForEvent;
using EventRegistrationSystem.Application.Features.Registrations.Queries.GetEventRegistrations;
using EventRegistrationSystem.Application.Features.Registrations.Queries.GetMyRegistrations;
using MediatR;
using EventRegistrationSystem.API.Authorization;
using EventRegistrationSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventRegistrationSystem.API.Controllers;

[Route("api")]
[ApiController]
public class RegistrationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegistrationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers the current user for a specific event
    /// </summary>
    [HttpPost("events/{eventId}/registrations")]
    [HasPermission(Permissions.Registrations.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterForEvent(Guid eventId)
    {
        var command = new RegisterForEventCommand(eventId);
        var registrationId = await _mediator.Send(command);
        return Created("", new { Id = registrationId });
    }

    /// <summary>
    /// Gets all registrations for the current user
    /// </summary>
    [HttpGet("registrations/me")]
    [HasPermission(Permissions.Registrations.ViewOwn)]
    [ProducesResponseType(typeof(List<EventRegistrationSystem.Application.Features.Registrations.DTOs.RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyRegistrations()
    {
        var query = new GetMyRegistrationsQuery();
        var registrations = await _mediator.Send(query);
        return Ok(registrations);
    }

    /// <summary>
    /// Cancels a specific registration
    /// </summary>
    [HttpDelete("registrations/{registrationId}")]
    [HasPermission(Permissions.Registrations.CancelOwn)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelRegistration(Guid registrationId)
    {
        var command = new CancelRegistrationCommand(registrationId);
        await _mediator.Send(command);
        return Ok(new { Message = "Your registration has been successfully cancelled. The capacity has been updated." });
    }

    /// <summary>
    /// Gets all registrations for a specific event (Organizer/Admin only)
    /// </summary>
    [HttpGet("events/{eventId}/registrations")]
    [HasPermission(Permissions.Registrations.ViewEvent)]
    [ProducesResponseType(typeof(List<EventRegistrationSystem.Application.Features.Registrations.DTOs.RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventRegistrations(Guid eventId)
    {
        var query = new GetEventRegistrationsQuery(eventId);
        var registrations = await _mediator.Send(query);
        return Ok(registrations);
    }
}
