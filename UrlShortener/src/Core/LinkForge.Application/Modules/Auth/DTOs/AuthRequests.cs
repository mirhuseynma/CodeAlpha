namespace LinkForge.Application.Modules.Auth.DTOs;

public record RegisterRequestDto(string Email, string Password, string ConfirmPassword, string FullName);
public record LoginRequestDto(string Email, string Password);
public record RefreshTokenRequestDto(string Email, string RefreshToken);
public record ConfirmEmailRequestDto(string Email, string Token);
public record ForgotPasswordRequestDto(string Email);
public record ResetPasswordRequestDto(string Email, string Token, string NewPassword);
