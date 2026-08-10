
namespace EventRegistrationSystem.Application.Features.Events.Commands.DeleteEvent;

public record DeleteEventCommand(Guid Id) : IRequest;
