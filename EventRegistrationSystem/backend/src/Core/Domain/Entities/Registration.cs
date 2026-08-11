using EventRegistrationSystem.Domain.Enums;

namespace EventRegistrationSystem.Domain.Entities;

public class Registration
{
    public Guid Id { get; set; }
    
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;
}
