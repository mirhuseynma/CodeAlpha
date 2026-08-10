namespace EventRegistrationSystem.Application.Features.Auth.DTOs;

public record AuthResult(
    string AccessToken,
    string RefreshToken
);
