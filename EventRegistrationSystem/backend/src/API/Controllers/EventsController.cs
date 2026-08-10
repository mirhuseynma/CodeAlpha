
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

    [HttpGet]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetEvents()
    {
        var result = await _mediator.Send(new GetEventsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Events.Create)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEventById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.Events.Update)]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id in route must match Id in body.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Events.Delete)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        await _mediator.Send(new DeleteEventCommand(id));
        return NoContent();
    }
}
