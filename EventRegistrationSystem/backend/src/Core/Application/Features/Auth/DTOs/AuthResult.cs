namespace EventRegistrationSystem.Application.Features.Auth.DTOs;

public record AuthResult(
    Guid UserId,
    string Email,
    string Token,
    string RefreshToken
);
