using MediatR;
using Microsoft.AspNetCore.Identity;
using EventRegistrationSystem.Domain.Entities;
using EventRegistrationSystem.Application.Exceptions;

namespace EventRegistrationSystem.Application.Features.Users.Commands.UpdateUserRole;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UpdateUserRoleCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {request.UserId} not found.");
        }

        var roleExists = await _roleManager.RoleExistsAsync(request.NewRole);
        if (!roleExists)
        {
            throw new BadRequestException($"Role '{request.NewRole}' does not exist.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains("Admin"))
        {
            throw new BadRequestException("Cannot update the role of an Admin user.");
        }

        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, request.NewRole);

        return Unit.Value;
    }
}
