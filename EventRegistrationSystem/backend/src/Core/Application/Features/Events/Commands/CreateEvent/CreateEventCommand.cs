
namespace EventRegistrationSystem.Application.Features.Events.Commands.CreateEvent;

public record CreateEventCommand(
    string Title,
    string Description,
    string Location,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity
) : IRequest<EventDto>;
