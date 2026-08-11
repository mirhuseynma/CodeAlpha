using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using EventRegistrationSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands.RegisterForEvent;

public class RegisterForEventCommandHandler : IRequestHandler<RegisterForEventCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RegisterForEventCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(RegisterForEventCommand request, CancellationToken cancellationToken)
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

        if (@event.StartDate <= DateTime.UtcNow)
        {
            throw new BadRequestException("Cannot register for an event that has already started.");
        }

        if (@event.Capacity <= 0)
        {
            throw new EventCapacityExceededException(request.EventId);
        }

        var isAlreadyRegistered = await _context.Registrations
            .AnyAsync(r => r.EventId == request.EventId && r.UserId == userId && r.Status == Domain.Enums.RegistrationStatus.Registered, cancellationToken);

        if (isAlreadyRegistered)
        {
            throw new RegistrationAlreadyExistsException(request.EventId, userId);
        }

        // Decrease capacity
        @event.Capacity -= 1;

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            UserId = userId,
            RegisteredAt = DateTime.UtcNow,
            Status = Domain.Enums.RegistrationStatus.Registered
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync(cancellationToken);

        return registration.Id;
    }
}
