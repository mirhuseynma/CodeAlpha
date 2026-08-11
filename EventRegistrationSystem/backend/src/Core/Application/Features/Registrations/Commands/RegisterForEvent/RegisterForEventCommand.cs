using MediatR;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands.RegisterForEvent;

public record RegisterForEventCommand(Guid EventId) : IRequest<Guid>;
