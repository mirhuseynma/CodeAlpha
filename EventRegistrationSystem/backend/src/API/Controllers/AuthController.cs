using EventRegistrationSystem.Application.Features.Auth.Commands.Login;
using EventRegistrationSystem.Application.Features.Auth.Commands.Refresh;
using EventRegistrationSystem.Application.Features.Auth.Commands.Register;

namespace EventRegistrationSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
