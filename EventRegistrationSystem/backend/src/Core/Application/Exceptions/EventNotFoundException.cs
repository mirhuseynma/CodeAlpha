namespace EventRegistrationSystem.Application.Exceptions;

public class EventNotFoundException : NotFoundException
{
    public EventNotFoundException(Guid eventId) 
        : base($"Event with id {eventId} was not found.")
    {
    }
}
