namespace EventRegistrationSystem.Application.Exceptions;

public class ForbiddenOperationException : ForbiddenException
{
    public ForbiddenOperationException(string message) 
        : base(message)
    {
    }
}
