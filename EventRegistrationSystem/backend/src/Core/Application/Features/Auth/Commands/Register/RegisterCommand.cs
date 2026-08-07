using EventRegistrationSystem.Application.Features.Auth.DTOs;

namespace EventRegistrationSystem.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<AuthResult>;
