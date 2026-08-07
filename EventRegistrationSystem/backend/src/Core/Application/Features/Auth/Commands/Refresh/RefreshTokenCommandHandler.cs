using EventRegistrationSystem.Application.Features.Auth.DTOs;
using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventRegistrationSystem.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenCommandHandler(UserManager<User> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // For simplicity in this demo, we assume the token is validated inside JwtProvider or similar.
        // Actually we need to extract claims from expired token. But let's simplify for Day 1 and just find user by token (if stored) or pass the user ID inside the refresh request.
        // Wait, standard way is to read the expired token. But we don't have access to token validation here easily without injecting it.
        // Instead, let's just use the RefreshToken to find the user.
        
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);
        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token."); // Custom exception
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _jwtProvider.GenerateToken(user, roles);
        
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResult(user.Id, user.Email, newToken, newRefreshToken);
    }
}
