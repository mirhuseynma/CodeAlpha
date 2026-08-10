
namespace EventRegistrationSystem.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateEventCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
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
            throw new EventRegistrationSystem.Application.Exceptions.ForbiddenException("You do not have permission to update this event because you are not the organizer.");
        }

        // Apply changes
        existingEvent.Title = request.Title;
        existingEvent.Description = request.Description;
        existingEvent.Location = request.Location;
        existingEvent.StartDate = request.StartDate;
        existingEvent.EndDate = request.EndDate;
        existingEvent.Capacity = request.Capacity;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        _context.Events.Update(existingEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return new EventDto
        {
            Id = existingEvent.Id,
            Title = existingEvent.Title,
            Description = existingEvent.Description,
            Location = existingEvent.Location,
            StartDate = existingEvent.StartDate,
            EndDate = existingEvent.EndDate,
            Capacity = existingEvent.Capacity,
            OrganizerId = existingEvent.OrganizerId,
            Status = existingEvent.Status,
            CreatedAt = existingEvent.CreatedAt
        };
    }
}

