
namespace EventRegistrationSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all events
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Events.View)]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEvents()
    {
        var result = await _mediator.Send(new GetEventsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific event by ID
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission(Permissions.Events.View)]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new event
    /// </summary>
    [HttpPost]
    [HasPermission(Permissions.Events.Create)]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEventById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing event
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission(Permissions.Events.Update)]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id in route must match Id in body.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an event
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.Events.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        await _mediator.Send(new DeleteEventCommand(id));
        return NoContent();
    }
}
