using MediatR;

namespace EventRegistrationSystem.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<Unit>;
