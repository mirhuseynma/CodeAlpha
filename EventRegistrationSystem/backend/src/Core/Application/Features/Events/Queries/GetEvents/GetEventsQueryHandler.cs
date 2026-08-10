
namespace EventRegistrationSystem.Application.Features.Events.Queries.GetEvents;

public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, List<EventDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetEventsQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Events.Include(e => e.Organizer).AsQueryable();

        var isUserAdmin = _currentUserService.IsAdmin;
        var isUserOrganizer = _currentUserService.IsOrganizer;

        // If user is Organizer (and not Admin), show only their events
        if (isUserOrganizer && !isUserAdmin)
        {
            var userIdString = _currentUserService.UserId;

            if (Guid.TryParse(userIdString, out var userId))
            {
                query = query.Where(e => e.OrganizerId == userId);
            }
        }

        var events = await query.OrderByDescending(e => e.CreatedAt).ToListAsync(cancellationToken);

        return events.Select(e => new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Capacity = e.Capacity,
            OrganizerId = e.OrganizerId,
            OrganizerName = e.Organizer.FirstName + " " + e.Organizer.LastName,
            Status = e.Status,
            CreatedAt = e.CreatedAt
        }).ToList();
    }
}

