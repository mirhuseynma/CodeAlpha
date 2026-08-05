using LinkForge.API.Authorization;
using LinkForge.Application.Modules.Admin.Queries;
using LinkForge.Application.Modules.Shortener.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAdminStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAdminUsersQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("links")]
    public async Task<IActionResult> GetLinks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAdminLinksQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("links/{id}")]
    public async Task<IActionResult> HardDeleteLink(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new HardDeleteShortLinkCommand(id), cancellationToken);
        return NoContent();
    }
}
