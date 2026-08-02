namespace LinkForge.Application.Common.Interfaces.Identity;

public interface IIdentityService
{
    Task<(bool Result, string UserId, string EmailConfirmationToken, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, string fullName);
    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<bool> CheckPasswordAsync(string email, string password);
    Task<(string Token, string RefreshToken)?> LoginAsync(string email, string password);
    Task<(string Token, string RefreshToken)> GenerateTokensAsync(string email);
    Task<(string Token, string RefreshToken)?> RefreshTokensAsync(string email, string refreshToken);
}
