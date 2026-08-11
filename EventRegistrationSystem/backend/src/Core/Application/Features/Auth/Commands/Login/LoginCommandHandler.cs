namespace EventRegistrationSystem.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(Microsoft.AspNetCore.Identity.UserManager<User> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new BadRequestException("Invalid email or password."); // TODO: Custom exception
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid email or password."); // TODO: Custom exception
        }

        //if (!await _userManager.IsEmailConfirmedAsync(user))
        //{
        //    throw new BadRequestException("Email is not confirmed. Please confirm your email before logging in.");
        //}

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtProvider.GenerateToken(user, roles);
        
        var refreshToken = _jwtProvider.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResult(token, refreshToken);
    }
}
