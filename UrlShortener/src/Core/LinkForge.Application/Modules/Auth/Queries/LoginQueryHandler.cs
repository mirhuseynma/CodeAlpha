namespace LinkForge.Application.Modules.Auth.Queries;

public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public LoginQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request.Email, request.Password);
        
        if (result == null)
        {
            throw new BadRequestException("Invalid credentials.");
        }

        return new AuthResponseDto(result.Value.Token, result.Value.RefreshToken);
    }
}
