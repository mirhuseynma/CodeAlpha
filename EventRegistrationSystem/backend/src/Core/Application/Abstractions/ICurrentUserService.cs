namespace EventRegistrationSystem.Application.Abstractions;

public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAdmin { get; }
    bool IsOrganizer { get; }
}
