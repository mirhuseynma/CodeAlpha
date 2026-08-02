namespace LinkForge.Application.Modules.Auth.DTOs;

public record AuthResponseDto(string AccessToken, string RefreshToken);
public record RegisterResponseDto(string EmailConfirmationToken);
