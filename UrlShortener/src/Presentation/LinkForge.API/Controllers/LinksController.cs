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

    [HttpPost]
    [EnableRateLimiting("IpRateLimit")]
    public async Task<IActionResult> Create([FromBody] CreateShortLinkRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateShortLinkCommand(request.OriginalUrl, request.CustomAlias);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }
}
