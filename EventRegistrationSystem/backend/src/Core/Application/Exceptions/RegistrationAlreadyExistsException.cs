namespace EventRegistrationSystem.Application.Exceptions;

public class RegistrationAlreadyExistsException : ConflictException
{
    public RegistrationAlreadyExistsException(Guid eventId, Guid userId) 
        : base($"User with ID {userId} is already registered for event with ID {eventId}.")
    {
    }
}
