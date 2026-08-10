
namespace EventRegistrationSystem.Application.Features.Events.Queries.GetEvents;

public record GetEventsQuery() : IRequest<List<EventDto>>;
