namespace EventRegistrationSystem.Application.Features.Registrations.DTOs;

public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventStartDate { get; set; }
    
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
