using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventRegistrationSystem.Application.Features.Users.DTOs;
using EventRegistrationSystem.Domain.Entities;

namespace EventRegistrationSystem.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly UserManager<User> _userManager;

    public GetUsersQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                roles.ToList()
            ));
        }

        return userDtos;
    }
}
