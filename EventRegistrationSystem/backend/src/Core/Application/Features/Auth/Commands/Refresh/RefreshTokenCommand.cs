
namespace EventRegistrationSystem.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResult>;
