
namespace EventRegistrationSystem.Application.Features.Events.Queries.GetEventById;

public record GetEventByIdQuery(Guid Id) : IRequest<EventDto>;
