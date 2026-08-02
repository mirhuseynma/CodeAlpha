namespace LinkForge.Application.Modules.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshTokensAsync(request.Email, request.RefreshToken);
        
        if (result == null)
        {
            throw new BadRequestException("Invalid or expired refresh token.");
        }

        return new AuthResponseDto(result.Value.Token, result.Value.RefreshToken);
    }
}
