namespace LinkForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[HasPermission(Permissions.ShortLinks.Create)]
public class LinksController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public LinksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string GetBaseUrl()
    {
        return $"{Request.Scheme}://{Request.Host}/";
    }

    [HttpPost]
    [EnableRateLimiting("IpRateLimit")]
    public async Task<IActionResult> Create([FromBody] CreateShortLinkRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateShortLinkCommand(request.OriginalUrl, request.CustomAlias, GetBaseUrl(), request.ExpiresAt);
        var result = await _mediator.Send(command, cancellationToken);
        
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetUserLinksQuery(pageNumber, pageSize, GetBaseUrl());
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var query = new GetUserStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        var query = new GetLinkByCodeQuery(code, GetBaseUrl());
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteShortLinkCommand(id);
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new ToggleLinkStatusCommand(id, request.IsActive);
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpGet("{id:guid}/analytics")]
    public async Task<IActionResult> GetAnalytics(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetLinkAnalyticsQuery(id, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }
}

public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}
