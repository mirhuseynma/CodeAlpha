namespace EventRegistrationSystem.Application.Features.Users.DTOs;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    List<string> Roles
);
