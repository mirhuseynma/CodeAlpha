using MediatR;
using EventRegistrationSystem.Application.Features.Users.DTOs;

namespace EventRegistrationSystem.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>;
