using LinkForge.Application.Common.Interfaces.Identity;
using LinkForge.Application.Modules.Admin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LinkForge.Persistence.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly TimeProvider _timeProvider;

    public IdentityService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator, TimeProvider timeProvider)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _timeProvider = timeProvider;
    }

    public async Task<(bool Result, string UserId, string EmailConfirmationToken, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, string fullName)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        var result = await _userManager.CreateAsync(user, password);
        string emailConfirmationToken = string.Empty;

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        return (result.Succeeded, user.Id.ToString(), emailConfirmationToken, result.Errors.Select(e => e.Description));
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<(string Token, string RefreshToken)?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        // if (!await _userManager.IsEmailConfirmedAsync(user))
        // {
        //     throw new BadRequestException("Email is not confirmed.");
        // }

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().AddDays(7); // 7 days expiry
        await _userManager.UpdateAsync(user);

        return (token, refreshToken);
    }

    public async Task<(string Token, string RefreshToken)> GenerateTokensAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new UnauthorizedException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().AddDays(7); // 7 days expiry
        await _userManager.UpdateAsync(user);

        return (token, refreshToken);
    }

    public async Task<(string Token, string RefreshToken)?> RefreshTokensAsync(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= _timeProvider.GetUtcNow())
        {
            return null; // Invalid or expired refresh token
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().AddDays(7);
        await _userManager.UpdateAsync(user);

        return (newAccessToken, newRefreshToken);
    }

    public async Task<int> GetTotalUsersAsync()
    {
        return await _userManager.Users.CountAsync();
    }

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
    {
        return await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserDto(
                u.Id,
                u.FullName,
                "", // LastName if you want to split FullName, or just leave empty
                u.Email ?? "",
                0, // LinksCount can be populated later or we leave it 0
                u.CreatedAt
            )).ToListAsync();
    }
}
