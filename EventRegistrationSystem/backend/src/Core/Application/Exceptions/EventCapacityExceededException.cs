namespace EventRegistrationSystem.Application.Exceptions;

public class EventCapacityExceededException : ConflictException
{
    public EventCapacityExceededException(Guid eventId) 
        : base($"Cannot register for event with ID {eventId} because its capacity is full.")
    {
    }
}
