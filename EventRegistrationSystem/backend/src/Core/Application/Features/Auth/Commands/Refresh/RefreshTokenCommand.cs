using EventRegistrationSystem.Application.Features.Auth.DTOs;

namespace EventRegistrationSystem.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<AuthResult>;
