
namespace EventRegistrationSystem.Application.Features.Events.Commands.DeleteEvent;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteEventCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new EventRegistrationSystem.Application.Exceptions.UnauthorizedException("User is not authenticated.");
        }

        var isUserAdmin = _currentUserService.IsAdmin;

        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        
        if (existingEvent == null)
        {
            throw new EventRegistrationSystem.Application.Exceptions.NotFoundException("Event not found.");
        }

        if (!isUserAdmin && existingEvent.OrganizerId != userId)
        {
            throw new EventRegistrationSystem.Application.Exceptions.ForbiddenException("You do not have permission to delete this event because you are not the organizer.");
        }

        _context.Events.Remove(existingEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

