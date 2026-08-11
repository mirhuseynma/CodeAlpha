using MediatR;
using Microsoft.AspNetCore.Identity;
using EventRegistrationSystem.Domain.Entities;
using EventRegistrationSystem.Application.Exceptions;

namespace EventRegistrationSystem.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public DeleteUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {request.UserId} not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            throw new BadRequestException("Cannot delete a user with the Admin role.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Failed to delete user.");
        }

        return Unit.Value;
    }
}
