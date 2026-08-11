using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using EventRegistrationSystem.Application.Features.Registrations.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventRegistrationSystem.Application.Features.Registrations.Queries.GetEventRegistrations;

public class GetEventRegistrationsQueryHandler : IRequestHandler<GetEventRegistrationsQuery, List<RegistrationDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetEventRegistrationsQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<RegistrationDto>> Handle(GetEventRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        var @event = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (@event == null)
        {
            throw new EventNotFoundException(request.EventId);
        }

        if (!_currentUserService.IsAdmin && @event.OrganizerId != userId)
        {
            throw new ForbiddenOperationException("You are not allowed to view registrations for an event you do not organize.");
        }

        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Where(r => r.EventId == request.EventId)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = @event.Title,
                EventStartDate = @event.StartDate,
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
