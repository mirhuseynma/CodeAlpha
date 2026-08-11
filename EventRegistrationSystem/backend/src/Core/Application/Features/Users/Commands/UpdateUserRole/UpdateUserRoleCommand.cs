using MediatR;

namespace EventRegistrationSystem.Application.Features.Users.Commands.UpdateUserRole;

public record UpdateUserRoleCommand(Guid UserId, string NewRole) : IRequest<Unit>;
