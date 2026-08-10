
namespace EventRegistrationSystem.Application.Features.Events.Commands.UpdateEvent;

public record UpdateEventCommand(
    Guid Id,
    string Title,
    string Description,
    string Location,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity
) : IRequest<EventDto>;
