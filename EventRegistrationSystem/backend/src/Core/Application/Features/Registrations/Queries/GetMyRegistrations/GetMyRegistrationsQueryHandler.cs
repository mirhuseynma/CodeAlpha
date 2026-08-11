using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using EventRegistrationSystem.Application.Features.Registrations.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventRegistrationSystem.Application.Features.Registrations.Queries.GetMyRegistrations;

public class GetMyRegistrationsQueryHandler : IRequestHandler<GetMyRegistrationsQuery, List<RegistrationDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyRegistrationsQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<RegistrationDto>> Handle(GetMyRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        var registrations = await _context.Registrations
            .Include(r => r.Event)
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = r.Event.Title,
                EventStartDate = r.Event.StartDate,
                UserId = r.UserId,
                UserFullName = r.User.FirstName + " " + r.User.LastName,
                UserEmail = r.User.Email ?? string.Empty,
                RegisteredAt = r.RegisteredAt,
                Status = r.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return registrations;
    }
}
