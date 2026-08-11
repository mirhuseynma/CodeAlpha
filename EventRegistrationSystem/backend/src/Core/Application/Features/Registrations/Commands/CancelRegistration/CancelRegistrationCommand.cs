using MediatR;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands.CancelRegistration;

public record CancelRegistrationCommand(Guid RegistrationId) : IRequest<Unit>;
