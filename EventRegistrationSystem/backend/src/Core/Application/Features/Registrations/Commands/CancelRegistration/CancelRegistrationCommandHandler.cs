using EventRegistrationSystem.Application.Abstractions;
using EventRegistrationSystem.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventRegistrationSystem.Application.Features.Registrations.Commands.CancelRegistration;

public class CancelRegistrationCommandHandler : IRequestHandler<CancelRegistrationCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelRegistrationCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CancelRegistrationCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        var registration = await _context.Registrations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == request.RegistrationId, cancellationToken);

        if (registration == null)
        {
            throw new NotFoundException($"Registration with ID {request.RegistrationId} not found.");
        }

        if (!_currentUserService.IsAdmin && registration.UserId != userId)
        {
            throw new ForbiddenOperationException("You are not allowed to cancel someone else's registration.");
        }

        // Increase capacity
        registration.Event.Capacity += 1;

        // We can either delete it or mark as cancelled. Marking as cancelled is safer for history.
        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
