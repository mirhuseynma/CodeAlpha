using EventRegistrationSystem.Application.Features.Users.Commands.DeleteUser;
using EventRegistrationSystem.Application.Features.Users.Commands.UpdateUserRole;
using EventRegistrationSystem.Application.Features.Users.DTOs;
using EventRegistrationSystem.Application.Features.Users.Queries.GetUsers;
using EventRegistrationSystem.API.Authorization;
using EventRegistrationSystem.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventRegistrationSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Users.ViewAll)]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    /// <summary>
    /// Updates a user's role
    /// </summary>
    [HttpPut("{id}/role")]
    [HasPermission(Permissions.Users.UpdateRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        await _mediator.Send(new UpdateUserRoleCommand(id, request.Role));
        return Ok(new { Message = "Role updated successfully." });
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
