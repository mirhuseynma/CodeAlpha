namespace LinkForge.API.Controllers;

[ApiController]
[Route("")]
public class RedirectController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUrlVisitQueue _visitQueue;

    public RedirectController(IMediator mediator, IUrlVisitQueue visitQueue)
    {
        _mediator = mediator;
        _visitQueue = visitQueue;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUrlByCodeQuery(code), cancellationToken);

        var visitEvent = new UrlVisitEventDto(
            response.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Request.Headers.UserAgent.ToString(),
            Request.Headers.Referer.ToString()
        );

        // Fire and forget to background channel (Zero-blocking analytics)
        await _visitQueue.EnqueueAsync(visitEvent, CancellationToken.None);

        return Redirect(response.OriginalUrl); // 302 Found
    }
}
