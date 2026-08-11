using EventRegistrationSystem.Application.Features.Registrations.DTOs;
using MediatR;

namespace EventRegistrationSystem.Application.Features.Registrations.Queries.GetMyRegistrations;

public record GetMyRegistrationsQuery : IRequest<List<RegistrationDto>>;
