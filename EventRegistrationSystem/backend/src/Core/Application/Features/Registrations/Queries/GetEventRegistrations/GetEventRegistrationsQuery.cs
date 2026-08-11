using EventRegistrationSystem.Application.Features.Registrations.DTOs;
using MediatR;

namespace EventRegistrationSystem.Application.Features.Registrations.Queries.GetEventRegistrations;

public record GetEventRegistrationsQuery(Guid EventId) : IRequest<List<RegistrationDto>>;
