using EventRegistrationSystem.Application.Features.Auth.DTOs;

namespace EventRegistrationSystem.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResult>;
