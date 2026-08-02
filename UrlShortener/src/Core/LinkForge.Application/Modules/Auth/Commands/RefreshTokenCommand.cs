namespace LinkForge.Application.Modules.Auth.Commands;

public record RefreshTokenCommand(string Email, string RefreshToken) : IRequest<AuthResponseDto>;
