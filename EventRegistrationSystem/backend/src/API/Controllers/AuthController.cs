
namespace EventRegistrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result)
        {
            return Ok(new { Message = "Email successfully confirmed. You can now login." });
        }
        
        return BadRequest(new { Message = "Failed to confirm email." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResult>> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
