namespace EventRegistrationSystem.Application.Features.Auth.DTOs;

public record RegisterResult(Guid UserId, string Message, string Token);
