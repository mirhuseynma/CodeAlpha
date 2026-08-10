namespace EventRegistrationSystem.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
    private readonly EventRegistrationSystem.Application.Abstractions.IJwtProvider _jwtProvider;

    public RefreshTokenCommandHandler(Microsoft.AspNetCore.Identity.UserManager<User> userManager, EventRegistrationSystem.Application.Abstractions.IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_userManager.Users, u => u.RefreshToken == request.RefreshToken, cancellationToken);
        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new EventRegistrationSystem.Application.Exceptions.BadRequestException("Invalid or expired refresh token."); // Custom exception
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _jwtProvider.GenerateToken(user, roles);
        
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResult(newToken, newRefreshToken);
    }
}
