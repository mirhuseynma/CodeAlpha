
namespace EventRegistrationSystem.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int Capacity { get; set; }
    
    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;
    
    public EventStatus Status { get; set; } = EventStatus.Upcoming;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
