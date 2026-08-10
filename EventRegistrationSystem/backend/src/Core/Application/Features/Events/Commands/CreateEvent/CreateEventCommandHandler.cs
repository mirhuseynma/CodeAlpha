namespace EventRegistrationSystem.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly EventRegistrationSystem.Application.Abstractions.IAppDbContext _context;
    private readonly EventRegistrationSystem.Application.Abstractions.ICurrentUserService _currentUserService;

    public CreateEventCommandHandler(EventRegistrationSystem.Application.Abstractions.IAppDbContext context, EventRegistrationSystem.Application.Abstractions.ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new EventRegistrationSystem.Application.Exceptions.UnauthorizedException("User is not authenticated or user id is invalid.");
        }

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            OrganizerId = userId,
            Status = EventRegistrationSystem.Domain.Enums.EventStatus.Upcoming,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return new EventDto
        {
            Id = newEvent.Id,
            Title = newEvent.Title,
            Description = newEvent.Description,
            Location = newEvent.Location,
            StartDate = newEvent.StartDate,
            EndDate = newEvent.EndDate,
            Capacity = newEvent.Capacity,
            OrganizerId = newEvent.OrganizerId,
            Status = newEvent.Status,
            CreatedAt = newEvent.CreatedAt
        };
    }
}


